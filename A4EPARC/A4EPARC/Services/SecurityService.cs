using System;
using System.Security.Cryptography;
using System.Text;

namespace A4EPARC.Services
{
    public class SecurityService : ISecurityService
    {
        public string Encrypt(string value)
        {
            var sb = new StringBuilder();

            using (SHA1 hash = SHA1Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        
        //public string RandomSalt(int size)
        //{
        //    var random = new Random((int)DateTime.Now.Ticks);
        //    var builder = new StringBuilder();
        //    for (var i = 0; i < size; i++)
        //    {
        //        char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
        //        builder.Append(ch);
        //    }

        //    return builder.ToString();
        //}

        public string RandomString(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            string input = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }

    public interface ISecurityService
    {
        string Encrypt(string value);
        string RandomString(int size);
    }


}
