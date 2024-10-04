using System.Security.Cryptography;
using System.Text;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.Models;

namespace Whistleblowing.NETAPI.Crypto
{
    public class CryptoService
    {
        public readonly WhistleBlowingContext _context;

        public CryptoService(WhistleBlowingContext context) { _context = context; }

        public CryptoKey fetchCryptoInfo()
        {
            return _context.CryptoKey.FirstOrDefault() as CryptoKey;
        }

        public void saveCryptoInfo( CryptoKey info)
        {
            _context.CryptoKey.Add(info);
            _context.SaveChanges();
        }

        //Viene richiamato nel momento in cui il db non ha valori di tipo CryptoKey.
        public static CryptoKey GenerateCryptoData(string password, int aesIterations, int aesKeySize)
        {
            using (RSA rsa = RSA.Create())
            {
                // Esportazione della chiave pubblica in formato Base64
                string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

                // Generazione del salt
                byte[] salt = GenerateSalt();

                // Esportazione della chiave privata e cifratura con la password dell'amministratore
                byte[] privateKeyBytes = rsa.ExportRSAPrivateKey();
                Console.WriteLine("Lunghezza private key: {0}", privateKeyBytes.Length);
                string encryptedPrivateKey = EncryptPrivateKeyWithPassword(privateKeyBytes, password, salt, aesIterations, aesKeySize);

                //Console.WriteLine("\n encrypted rsa private key: {0}\n\n", encryptedPrivateKey);
                //Console.WriteLine("rsa public key: {0}\n\n", publicKey);
                //Console.WriteLine("salt lenght e saltB64: {0} - {1}\n\n", salt.Length, Convert.ToBase64String(salt));
                //Console.WriteLine("aes key size: {0}\n\n", aesKeySize);
                //Console.WriteLine("aes iterations: {0}\n\n", aesIterations);

                // Salva i dati crittografici nel database
                return new CryptoKey
                {
                    EncryptedRsaPrivateKey = encryptedPrivateKey,
                    RsaPublicKey = publicKey,
                    Salt = Convert.ToBase64String(salt),
                    AesKeySize = aesKeySize,  // Dimensione chiave AES in bit
                    AesIterations = aesIterations  // Numero di iterazioni PBKDF2
                };
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];  // 128 bit di salt
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);  // Riempie il salt con byte casuali
            }
            return salt;
        }

        public static RSAParameters LoadPublicKey(string publicKey)
        {
            if (publicKey == null)
                throw new Exception("Chiave pubblica non trovata!");

            // Importa la chiave pubblica nel formato RSAParameters
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                return rsa.ExportParameters(false);
            }
        }

        public static RSAParameters LoadPrivateKey(string adminPassword, string privateKeyEncrypted, string saltString,int aesIterations, int aesKeySize)
        {
            if (privateKeyEncrypted == null)
                throw new Exception("Chiave privata non trovata!");
            if (saltString == null)
                throw new Exception("Salt non trovato!");

            byte[] salt = Convert.FromBase64String(saltString);

            // Decifrare la chiave privata usando la password dell'amministratore
            byte[] privateKeyBytes = DecryptPrivateKeyWithPassword(privateKeyEncrypted, adminPassword, salt, aesIterations, aesKeySize);
            Console.WriteLine(privateKeyBytes.Length);
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                return rsa.ExportParameters(true);
            }
        }

        private static string EncryptPrivateKeyWithPassword(byte[] privateKey, string password, byte[] salt, int iterations, int aesKeySize)
        {
            if (salt.Length == 0)
                throw new Exception("Salt non trovato!");

            Console.WriteLine($"Lunghezza chiave privata prima della cifratura: {privateKey.Length}");

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] key = deriveBytes.GetBytes(aesKeySize / 8);  // Ottieni la chiave AES in byte

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();  // Generazione dell'IV

                    byte[] encryptedKey;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(privateKey, 0, privateKey.Length);
                        cs.FlushFinalBlock();
                        encryptedKey = ms.ToArray();
                    }

                    Console.WriteLine($"IV - chiave privata cifrata: {Convert.ToBase64String(aes.IV)} - lunghezza: {encryptedKey.Length}");
                    return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(encryptedKey)}";  // Ritorna IV e chiave cifrata
                }
            }
        }

        private static byte[] DecryptPrivateKeyWithPassword(string encryptedPrivateKey, string password, byte[] salt, int aesIterations, int aesKeysize)
        {
            var parts = encryptedPrivateKey.Split(':');
            byte[] iv = Convert.FromBase64String(parts[0]);  // Estrai IV
            byte[] encryptedKeyBytes = Convert.FromBase64String(parts[1]);  // Estrai chiave cifrata

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, aesIterations, HashAlgorithmName.SHA256))
            {
                byte[] key = deriveBytes.GetBytes(aesKeysize / 8);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(encryptedKeyBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var resultStream = new MemoryStream())
                    {
                        cs.CopyTo(resultStream); // Copia tutto il contenuto dal CryptoStream al MemoryStream
                        return resultStream.ToArray(); // Ritorna la chiave privata decifrata
                    }
                }
            }
        }

        public static byte[] EncryptWithRSA(RSAParameters publicKey, string data)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                return rsa.Encrypt(Encoding.UTF8.GetBytes(data), RSAEncryptionPadding.OaepSHA256);
            }
        }

        public static string DecryptWithRSA(RSAParameters privateKey, byte[] encryptedData)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(privateKey);
                byte[] decryptedBytes = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        public static void PrintCryptoDataToConsole(CryptoKey data)
        {
            Console.WriteLine("\n encrypted rsa private key: {0}\n\n", data.EncryptedRsaPrivateKey);
            Console.WriteLine("rsa public key: {0}\n\n", data.RsaPublicKey);
            Console.WriteLine("saltB64: {0}\n\n", data.Salt);
            Console.WriteLine("aes key size: {0}\n\n", data.AesKeySize);
            Console.WriteLine("aes iterations: {0}\n\n", data.AesIterations);
        }
    }
}
