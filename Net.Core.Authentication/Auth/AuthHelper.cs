using System;
using System.Security.Cryptography;
using System.Text;

namespace Net.Core.Authentication.Auth
{
    public class AuthHelper
    {
        public static string ParseFromBase64(string text)
        {
            byte[] data = Decrypt(text);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }

        public static byte[] Decrypt(string signature)
        {
            return Convert.FromBase64String(signature);
        }

        public static string Encrypt(string signature)
        {
            byte[] b = Encoding.UTF8.GetBytes(signature);
            return Convert.ToBase64String(b);
        }

        public static string GetHash(string text, string key)
        {
            string passwordHash = string.Empty;
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {

                passwordHash = ByteToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(text)));
                
            }
            return passwordHash;
        }

        public static bool Validate(string data, string signature, string key)
        {
            string sig = Encoding.UTF8.GetString(Decrypt(signature));
            signature = GetHash(data, key);
            return sig.Equals(signature);
        }

        private static string ByteToString(byte[] buff)
        {
            StringBuilder hex = new StringBuilder(buff.Length * 2);
            foreach (byte b in buff)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

            //string sbinary = "";

            //for (int i = 0; i < buff.Length; i++)
            //{
            //    sbinary += buff[i].ToString("x2"); // hex format x2 lowercase, X2 uppercase
            //}
            //return (sbinary);
        }
    }
}
