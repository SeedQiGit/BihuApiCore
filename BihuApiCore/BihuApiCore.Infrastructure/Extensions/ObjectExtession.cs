using System;
using System.ComponentModel;

namespace BihuApiCore.Infrastructure.Extensions
{
    public static class ObjectExtession
    {
        #region 数据库值转换为对应类型

        /// <summary>
        /// 判断值类型,string,datetime
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static bool IsValueType(Type tp)
        {
            return tp.IsValueType || tp == typeof(Nullable<>) ||tp==typeof(Enum)|| tp == typeof(string) || tp == typeof(DateTime);
        }
 
        /// <summary>
        /// 数据库值转换为对应类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType">类型</param>
        /// <returns></returns>
        public static object DbChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (IsNullOrDbNull(value))
                    return null;
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            //枚举值赋值需要特殊转换
            if (conversionType.IsEnum)
            {
                //数字转换为枚举  ToObject还有很多其他重载，可以自己看
                return Enum.ToObject(conversionType, value);
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 空值判断 使用DbDataReader时候需要用这个判断
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDbNull(object obj)
        {
            return obj == null || obj is DBNull;
        }

        #endregion
       

        /// <summary>
        /// 更加安全的调用对象的ToString方法，如果是null，返回string.Empty；其它情况调用实际的ToString。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringNullToEmpty(this object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

    }
}
