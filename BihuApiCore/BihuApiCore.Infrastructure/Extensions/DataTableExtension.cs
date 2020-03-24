using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace BihuApiCore.Infrastructure.Extensions
{

    public static class DataTableExtension
    {
        /// <summary>
        /// 转换DataTable到IList强类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="bindLogic">绑定逻辑</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table, Action<T, DataRow> bindLogic)
        {
            return table == null
                ? new List<T>(0)
                : (from DataRow row in table.Rows select row.ToT<T>().DoFunction(row, bindLogic)).ToList();
            //DoFunction传入DataRow row对模型进行再次特殊处理（针对不能自动绑定的属性）
        }

        /// <summary>
        /// 转换DataTable到List类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table)
        {
            return table.ToList<T>(null);
        }

        /// <summary>
        ///  DataRow转换为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToT<T>(this DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }

            //值类型直接返回第一列
            Type tp = typeof(T);
            if (ObjectExtession.IsValueType(tp))
            {
                return (T)ObjectExtession.DbChangeType(row[0],tp);
            }

            T tReturn = Activator.CreateInstance<T>();

            //属性列表
            var properties = tp.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    object objValue = row[property.Name];
                  
                    //忽略空值,忽略只读属性
                    if (!ObjectExtession.IsNullOrDbNull(objValue) && property.CanWrite)
                    {
                        property.SetValue(tReturn, ObjectExtession.DbChangeType(objValue, property.PropertyType));
                    }
                }
            }
            return tReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="t1Value"></param>
        /// <param name="bindLogic">对于DataRow中需要进行特殊处理的字段进行再次赋值</param>
        /// <returns></returns>
        public static T DoFunction<T>(this T value, DataRow t1Value, Action<T, DataRow> bindLogic)
        {
            if (bindLogic == null)
                return value;
            bindLogic(value, t1Value);
            return value;
        }

       
    }
}
