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
    }
}
