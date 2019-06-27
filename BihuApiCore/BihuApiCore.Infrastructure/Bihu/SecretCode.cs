using BihuApiCore.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BihuApiCore.Infrastructure.Bihu
{
    /// <summary>
    /// 加密码扩展方法
    /// </summary>
    public static class SecretCodeHelper
    {
        /// <summary>
        /// 这里可以写个扩展，把所有属性自动过滤掉SecretCode属性。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlMd5(this string url)
        {
            string[] arr = url.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            //排序  这里就根据属性顺序赋值吧，不然反射也没法相同顺序
            //Array.Sort(arr);
            return string.Join("&", arr).GetMd5();
        }

        /// <summary>
        /// 获取加密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetSecretCode (this ISecretCode entity)
        {
            Type etype = entity.GetType();
            PropertyInfo[] props = etype.GetProperties();
            
            List<string> strList=new List<string>();
            //遍历实体属性进行赋值
            foreach (PropertyInfo pi in props)
            {
                var fieldName = pi.Name;
                if (fieldName=="SecretCode")
                {
                    continue;
                }
                string str=fieldName;
                //非值类型，跳过 
                if (!ObjectExtession.IsValueType(pi.PropertyType)) continue; 
                var piValue = pi.GetValue(entity);
                if (piValue==null)
                {
                    continue;
                }
                else
                {
                    str += "=" + piValue;
                }
                strList.Add(str);
            }
            return string.Join("&", strList).GetMd5();
        }
    }
}
