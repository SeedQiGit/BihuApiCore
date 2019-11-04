using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Infrastructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BihuApiCore.Infrastructure.Helper.Http
{
    public static class QueryStringHelper
    {
         public static string PostSecCode<T>(this T source) where T : new()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = source.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return "".GetMd5().ToLower();
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                if (item.Name.ToLower() != "seccode" && (item.PropertyType == typeof(String)
                                                         || item.PropertyType == typeof(Int32)
                                                         || item.PropertyType == typeof(Int64)
                                                         || item.PropertyType == typeof(float)
                                                         || item.PropertyType == typeof(Double))
                )
                {
                    string name = item.Name;
                    var obj = item.GetValue(source, null);
                    if (obj != null)
                    {
                        var val = JsonConvert.SerializeObject(obj);
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            dic.Add(name, obj.ToString());
                        }
                    }
                }
            }
            var tmp = dic.Where(x => !string.IsNullOrWhiteSpace(x.Value)).OrderBy(y => y.Key);
            string result = string.Join("&", tmp.Select(p => p.Key + '=' + p.Value).ToArray());
            return result.GetMd5().ToLower();
        }


        /// <summary>
        /// 简单的获取get请求参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string QueryStringSimple(this Object request)
        {
            StringBuilder query = new StringBuilder("?");
            PropertyInfo[] propertys = request.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                if (pi.CanRead)
                {
                    query.Append($@"{pi.Name}={pi.GetValue(request)}&");
                }
            }
            query = query.Remove(query.Length - 1, 1);
            return query.ToString();
        }


        /// <summary>
        ///     keyCode用于调用服务端接口，保证安全性
        /// </summary>
        private static string KeyCode = ConfigurationManager.GetValue("KeyCode");

        /// <summary>
        /// 字典转请求参数 
        /// </summary>
        /// <param name="parms">参数列表</param>
        /// <param name="withSecCode">是否加SecCode加密串</param>
        /// <returns>?开头参数串，或空字符串</returns>
        public static string QueryString(this IDictionary<string, object> parms, bool withSecCode = false)
        {
            var keys = parms.Keys;
            List<string> pars = new List<string>();
            foreach (var key in keys)
            {
                string name = key;
                object value = parms[key];
                //忽略空值
                if (value == null || string.IsNullOrEmpty(value.ToString())) continue;
                var vtype = value.GetType();
                //数组或list，参数遍历
                if (vtype.IsArray || vtype.IsGenericType
                    && (vtype.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)) ||
                        vtype.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerator))))
                {
                    var enumerator = vtype.IsArray || vtype.IsGenericType &&
                             vtype.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable))
                        ? (value as IEnumerable).GetEnumerator()
                        : (value as IEnumerator);

                    while (enumerator != null && enumerator.MoveNext())
                    {
                        pars.Add($"{name}={enumerator.Current}");
                    }
                }
                else
                {
                    pars.Add($"{name}={value}");

                }
            }

            var parstr = new StringBuilder(string.Join('&', pars));
            if (!withSecCode)
            {
                if (parstr.Length > 0)
                    parstr.Insert(0, '?');
                return parstr.ToString();
            }
            
            parstr.AppendFormat($"{(parstr.Length == 0 ? "" : "&")}");

            var secCode = parstr.ToString().GetMd5();
            parstr.AppendFormat("&SecCode={0}", secCode);
            parstr.Insert(0, '?');
            return parstr.ToString();
        }

        /// <summary>
        ///     实体转请求参数，忽略空属性、字段
        /// </summary>
        /// <param name="obj">参数实体</param>
        /// <param name="withSecCode">是否加SecCode加密串</param>
        /// <returns>?开头参数串，或空字符串</returns>
        public static string QueryString<T>(this T obj, bool withSecCode = false) where T : class
        {
            var parms = Obj2Dictionary(obj);
            return parms.QueryString(withSecCode);
        }

        /// <summary>
        ///  实体转参数字典
        /// </summary>
        /// <typeparam name="T">实体类别</typeparam>
        /// <param name="obj">实体</param>
        /// <returns></returns>
        private static IDictionary<string, object> Obj2Dictionary<T>(T obj) where T : class
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IDictionary<string, object> parms = new Dictionary<string, object>();
            foreach (PropertyInfo pair in properties)
            {
                object value = pair.GetValue(obj, null);
                ;
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    continue;
                }

                parms.Add(pair.Name, value);
            }

            var fields =
                type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo pair in fields)
            {
                object value = pair.GetValue(obj);
                ;
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    continue;
                }

                parms.Add(pair.Name, value);
            }

            return parms;
        }
    }
}
