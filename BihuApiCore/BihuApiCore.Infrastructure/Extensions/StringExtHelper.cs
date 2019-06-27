using System.Security.Cryptography;
using System.Text;

namespace BihuApiCore.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string GetMd5(this string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                byte[] md5Bytes = md5.ComputeHash(bytes);
                foreach (byte item in md5Bytes)
                {
                    stringBuilder.Append(item.ToString("x2"));
                }
            }
            return stringBuilder.ToString();

        }
    }
}
