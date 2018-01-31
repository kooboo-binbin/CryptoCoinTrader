using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptoCoinTrader.Manifest.Helpers
{
    public static class CryptoHelper
    {
        public static string HmacSha256Base64(string secret, string message)
        {
            var key = Convert.FromBase64String(secret);
            return Convert.ToBase64String(new HMACSHA256(key).ComputeHash(Encoding.UTF8.GetBytes(message)));
        }


        public static string HmacSha256Hex(string secret, string message)
        {
            var key = Encoding.ASCII.GetBytes(secret);
            var hmacs = new HMACSHA256(key);
            var hash = hmacs.ComputeHash(Encoding.ASCII.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToUpper();

        }
    }
}
