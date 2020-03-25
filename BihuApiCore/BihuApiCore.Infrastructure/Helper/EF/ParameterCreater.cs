using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace BihuApiCore.Infrastructure.Helper.EF
{
    public class ParameterCreater
    {
        #region 常量

        /// <summary>
        ///     根据配置文件中所配置的数据库类型
        ///     来获取命令参数中的参数符号oracle为":",mysql为"?"
        /// </summary>
        /// <returns></returns>
        public static string DbParmCharacter => "@";

        #endregion

        #region 创建参数

        /// <summary>
        ///     根据配置文件中所配置的数据库类型
        ///     来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter Create()
        {
            return new MySqlParameter();
        }

        /// <summary>
        ///     根据配置文件中所配置的数据库类型
        ///     来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter Create(string paramName, object value)
        {
            DbParameter param = Create();
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        ///     根据配置文件中所配置的数据库类型
        ///     来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter Create(string paramName, object value, DbType dbType)
        {
            DbParameter param = Create();
            param.DbType = dbType;
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        #endregion

        #region 对象参数转换DbParameter

        /// <summary>
        ///     转换对应的数据库参数
        /// </summary>
        /// <param name="dbParameter">参数</param>
        /// <returns></returns>
        public static DbParameter[] GetParameter(DbParameter[] dbParameter)
        {
            int i = 0;
            int size = dbParameter.Length;
            DbParameter[] result = new MySqlParameter[size];
            while (i < size)
            {
                result[i] = new MySqlParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                i++;
            }

            return result;
        }


        /// <summary>
        ///     对象参数转换DbParameter
        /// </summary>
        /// <returns></returns>
        public static DbParameter[] GetParameter<T>(T entity)
        {
            IList<DbParameter> parameter = new List<DbParameter>();
            DbType dbtype = new DbType();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                object value = pi.GetValue(entity, null);
                if (value == null || value == DBNull.Value)
                {
                    continue;
                }

                if (value.ToString() == "&nbsp;")
                {
                    value = null;
                }

                if (pi.PropertyType.ToString() == "System.DateTime" ||
                    pi.PropertyType.ToString() == "System.Nullable`1[System.DateTime]")
                {
                    if (Convert.ToDateTime(value) == DateTime.MinValue)
                    {
                        value = null;
                    }
                }

                switch (pi.PropertyType.ToString())
                {
                    case "System.Byte":
                        dbtype = DbType.Byte;
                        break;
                    case "System.Int32":
                    case "System.Nullable`1[System.Int32]":
                        dbtype = DbType.Int32;
                        break;
                    case "System.Int64":
                    case "System.Nullable`1[System.Int64]":
                        dbtype = DbType.Int64;
                        break;
                    case "System.Decimal":
                    case "System.Nullable`1[System.Decimal]":
                        dbtype = DbType.Decimal;
                        break;
                    case "System.DateTime":
                    case "System.Nullable`1[System.DateTime]":
                        dbtype = DbType.DateTime;
                        break;
                    default:
                        dbtype = DbType.String;
                        break;
                }

                parameter.Add(Create(DbParmCharacter + pi.Name, value, dbtype));
            }

            return parameter.ToArray();
        }

        /// <summary>
        ///     对象参数转换DbParameter
        /// </summary>
        /// <returns></returns>
        public static DbParameter[] GetParameter(Hashtable ht)
        {
            IList<DbParameter> parameter = new List<DbParameter>();
            DbType dbtype = new DbType();
            foreach (string key in ht.Keys)
            {
                if (ht[key] is DateTime)
                    dbtype = DbType.DateTime;
                else
                    dbtype = DbType.String;
                parameter.Add(Create(DbParmCharacter + key, ht[key], dbtype));
            }

            return parameter.ToArray();
        }

        #endregion
    }
}
