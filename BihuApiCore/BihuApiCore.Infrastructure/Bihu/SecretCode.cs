using System;
using System.Security.Cryptography;
using System.Text;

namespace BihuApiCore.Infrastructure.Bihu
{
    public class SecretCode
    {
        public void Test()
        {
            var str = "123456&1231231";
            if (str.GetUrl().GetMd5() == str)
            {

            }
        }
    }


    public static class SecretCodeHelper
    {
        public static string GetUrl(this string url)
        {
            string[] arr = url.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(arr);
            return string.Join("&", arr);
        }

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
