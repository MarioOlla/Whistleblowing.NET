using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NETAPI.Models
{
	public class CryptoKey
	{
		[Key]
		[Column("crypto_id")]
		public int Id { get; set; }


		// Chiave privata RSA cifrata con la password dell'amministratore
		[Column("EncryptedRsaPrivateKey")]
		public string EncryptedRsaPrivateKey { get; set; }
		[Column("RsaPublicKey")]
		// Chiave pubblica RSA
		public string RsaPublicKey { get; set; }
		[Column("Salt")]

		// Salt utilizzato per la derivazione della chiave AES
		public string Salt { get; set; }
		[Column("AesKeySize")]

		// Dimensione della chiave AES (in bit)
		public int AesKeySize { get; set; }
		[Column("AesIterations")]

		// Numero di iterazioni utilizzate per la derivazione della chiave AES
		public int AesIterations { get; set; }
	}
}
