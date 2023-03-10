using System;
using System.Security.Cryptography;

namespace Net.Core.Authentication.Auth
{
    public class KeyGen
    {
        private static readonly int keyLength = 16;
        public static string Base64()
        {
            return Convert.ToBase64String(GenerateBytes(keyLength));
        }

        public static string Number(int n)
        {
            Random generator = new Random();
            return generator.Next(0, 1000000).ToString(string.Format("D{0}",n));
        }

        private static byte[] GenerateBytes(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return randomBytes;
        }
    }
}
