using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Whistleblowing.NETAPI.Data;
using Whistleblowing.NETAPI.Models;

namespace Whistleblowing.NETAPI.Crypto
{
    public static class CryptoUtils
    {
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
                string encryptedPrivateKey = EncryptPrivateKeyWithPassword(privateKeyBytes, password, salt, aesIterations, aesKeySize);

                // Salva i dati crittografici nel database
                return new CryptoKey
                {
                    EncryptedRsaPrivateKey = encryptedPrivateKey,
                    RsaPublicKey = publicKey,
                    Salt = Convert.ToBase64String(salt),
                    AesKeySize = 256,  // Dimensione chiave AES in bit
                    AesIterations = 10000  // Numero di iterazioni PBKDF2
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

        public static RSAParameters LoadPrivateKey(string adminPassword, string privateKeyEncrypted, string saltString, int aesKeySize)
        {
            if (privateKeyEncrypted == null)
                throw new Exception("Chiave privata non trovata!");
            if (saltString == null)
                throw new Exception("Salt non trovato!");

            byte[] salt = Convert.FromBase64String(saltString);

            // Decifrare la chiave privata usando la password dell'amministratore
            byte[] privateKeyBytes = DecryptPrivateKeyWithPassword(privateKeyEncrypted, adminPassword, salt);

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

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations , HashAlgorithmName.SHA256))
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

                    return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(encryptedKey)}";  // Ritorna IV e chiave cifrata
                }
            }
        }

        private static byte[] DecryptPrivateKeyWithPassword(string encryptedPrivateKey, string password, byte[] salt)
        {
            var parts = encryptedPrivateKey.Split(':');
            byte[] iv = Convert.FromBase64String(parts[0]);  // Estrai IV
            byte[] encryptedKeyBytes = Convert.FromBase64String(parts[1]);  // Estrai chiave cifrata

            using (var context = new WhistleBlowingContext(new DbContextOptions<WhistleBlowingContext>()))
            {
                var cryptoKey = context.CryptoKey.FirstOrDefault();
                if (cryptoKey == null)
                    throw new Exception("CryptoKey non trovato!");

                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, cryptoKey.AesIterations, HashAlgorithmName.SHA256))
                {
                    byte[] key = deriveBytes.GetBytes(cryptoKey.AesKeySize / 8);

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        byte[] decryptedKey;

                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        using (var ms = new MemoryStream(encryptedKeyBytes))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            decryptedKey = new byte[encryptedKeyBytes.Length];
                            int decryptedByteCount = cs.Read(decryptedKey, 0, decryptedKey.Length);
                            return decryptedKey.Take(decryptedByteCount).ToArray();  // Ritorna la chiave privata decifrata
                        }
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
    }
}
