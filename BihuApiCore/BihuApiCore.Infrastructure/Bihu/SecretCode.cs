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
                //KeyCode是一个字符串密钥
                var strUrl = string.Format("Name={0}&Pwd={1}&CustKey={2}&KeyCode={3}", "", "", "","asdasd");
            }
        }
    }


    public static class SecretCodeHelper
    {
        /// <summary>
        /// 这里可以写个扩展，把所有属性自动过滤掉SecretCode属性。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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
