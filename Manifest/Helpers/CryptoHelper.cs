using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptoCoinTrader.Manifest.Helpers
{
    public static class CryptoHelper
    {
        public static string HmacSha256(string message, byte[] key)
        {
            return Convert.ToBase64String(new HMACSHA256(key).ComputeHash(Encoding.UTF8.GetBytes(message)));
        }

    }
}
