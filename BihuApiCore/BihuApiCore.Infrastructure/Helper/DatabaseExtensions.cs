using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace BihuApiCore.Infrastructure.Helper
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 网上找到的方法  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> SqlQuery<T>(this DbContext db, string sql, params object[] parameters)
            where T : new()
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = db.Database.GetDbConnection();
            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);
                    var propts = typeof(T).GetProperties();
                    var rtnList = new List<T>();
                    T model;
                    object val;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new T();
                            foreach (var l in propts)
                            {
                                val = reader[l.Name];
                                l.SetValue(model, val == DBNull.Value ? null : val);
                            }
                            rtnList.Add(model);
                        }
                    }
                    return rtnList;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// EF Core使用原生SQL的扩展方法   除了这个方法，其他方法在v盟中都没什么引用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> SqlQueryExt<T>(this DatabaseFacade database, string sql, params object[] parameters)
            where T : new()
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddRange(GetSqlParameters(parameters));
                    var propts = typeof(T).GetProperties();
                    var rtnList = new List<T>();
                    T model;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new T();
                            foreach (var propt in propts)
                            {
                                if (propt.CanWrite)
                                {
                                    
                                    object value = reader[propt.Name];
                                    if (value is DBNull) propt.SetValue(model, null);
                                    else propt.SetValue(model, value);
                                    //原来v盟的写法，不知道为啥这么写
                                    //if (reader.ReaderExists(propt.Name))
                                    //{
                                    //    value = reader[propt.Name];
                                    //    if (value is DBNull) propt.SetValue(model, null);
                                    //    else propt.SetValue(model, value);
                                    //}
                                }
                            }
                            rtnList.Add(model);
                        }
                    }
                    return rtnList;
                }
            }
            catch (Exception ex) {
                LogHelper.Error("查询数据错误",ex);
            }
            finally
            {
                conn.Close();
            }
            return new List<T>();
        }

        /// <summary>
        /// 获取第一行数据（这么做相比于SqlQueryExt，唯一的好处就是节约内存了，如果是多行数据的情况下，list的多条内存，变为单条内存）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T SqlQuerySingle<T>(this DatabaseFacade database, string sql, params object[] parameters)
           where T : new()
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddRange(GetSqlParameters(parameters));
                    var propts = typeof(T).GetProperties();
                    //var rtnList = new List<T>();
                    T model;
                    using (var reader = command.ExecuteReader())
                    {
                        model = new T();
                        while (reader.Read())
                        {
                            foreach (var propt in propts)
                            {
                                if (propt.CanWrite)
                                {
                                    object value;
                                    if (reader.ReaderExists(propt.Name))
                                    {
                                        value = reader[propt.Name];
                                        if (value is DBNull) propt.SetValue(model, null);
                                        else propt.SetValue(model, value);
                                    }
                                }
                            }
                        }
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("查询数据错误：", ex);
            }
            finally
            {
                conn.Close();
            }
            return default(T);
        }

        /// <summary>
        /// 判断DbDataReader是否存在某列
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        static private bool ReaderExists(this DbDataReader dr, string columnName)
        {
            int count = dr.GetColumnSchema().Where(x => x.ColumnName.ToUpper() == columnName.ToUpper()).Count();
            return count > 0;
        }

        /// <summary>
        /// EfCore执行原生sql语句，以DbDataReader返回结果，在map中做数据匹配
        /// </summary>
        /// <typeparam name="T">数据结果类型</typeparam>
        /// <param name="context">DatabaseFacade对象</param>
        /// <param name="query">sql语句</param>
        /// <param name="map">用于DbDataReader 和 对象之间匹配的</param>
        /// <returns></returns>
        public static List<T> ExecSQL<T>(this DatabaseFacade context, string query, Func<DbDataReader, T> map, params object[] parameters)
        {
            try
            {
                using (var reader = PreCommandReader(context, query, GetSqlParameters(parameters)))
                {
                    var entities = new List<T>();
                    while (reader.Read())
                    {
                        entities.Add(map(reader));
                    }

                    return entities;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format(@"ExecSQL:执行sql\r\n{0}出现错误", query), ex);
            }

        }

        /// <summary>
        /// 执行sql语句返回List集合(适合简单对象)
        /// </summary>
        /// <typeparam name="T">集合数据类型</typeparam>
        /// <param name="context">DatabaseFacade 对象</param>
        /// <param name="query">sql语句,需要完整的语句(包含参数)</param>
        /// <remarks>此方法中用的反射，自动填充数据</remarks>
        /// <returns></returns>
        public static List<T> ExecSQL<T>(this DatabaseFacade context, string query, params object[] parameters)
        {
            try
            {
                using (var result = PreCommandReader(context, query, GetSqlParameters(parameters)))
                {
                    List<T> list = new List<T>();
                    T obj = default(T);
                    while (result.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (!object.Equals(result[prop.Name], DBNull.Value))
                            {
                                prop.SetValue(obj, result[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format(@"ExecSQL:执行sql\r\n{0}出现错误", query), ex);
            }

        }


        /// <summary>
        /// 执行原生语句相当于原生ExecuteScalar
        /// </summary>
        /// <param name="context"></param>
        /// <param name="query">sql语句</param>
        /// <returns></returns>
        public static object ExecuteScalar(this DatabaseFacade context, string query )
        {
            try
            {
                using (var command = context.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    context.OpenConnection();
                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行ExecuteScalar语句出现异常", ex);
            }
        }

        /// <summary>
        /// 执行原生语句相当于原生ExecuteScalar
        /// </summary>
        /// <param name="context"></param>
        /// <param name="query">sql语句</param>
        /// <returns></returns>
        public static object ExecuteScalar(this DatabaseFacade context, string query, params object[] parameters)
        {

            try
            {
                using (var command = context.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    if (parameters != null)
                        command.Parameters.AddRange(GetSqlParameters(parameters));
                    command.CommandType = CommandType.Text;
                    context.OpenConnection();
                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行ExecuteScalar语句出现异常", ex);
            }
        }

        #region 辅助方法

        private static DbDataReader PreCommandReader(DatabaseFacade context, string query, params object[] parameters)
        {
            try
            {
                using (var command = context.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    if (parameters != null)
                        command.Parameters.AddRange(GetSqlParameters(parameters));
                    command.CommandType = CommandType.Text;

                    context.OpenConnection();

                    return command.ExecuteReader();
                }

            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("PreCommandReader执行sql:{0}", query), ex);
            }
            finally
            {
                context.CloseConnection();
            }


        }
        #endregion

        #region 由于sql参数化导致引用冲突转换参数

        public static MySqlParameter[] GetSqlParameters(object[] Parameters)
        {
            List<MySqlParameter> parames = new List<MySqlParameter>();
            if (Parameters != null)
            {
                foreach (var item in Parameters)
                {
                    List<(string, object)> str = item.GetType().GetProperties().Select(x => (x.Name, x.GetValue(item, null))).ToList();
                    foreach (var properties in str)
                    {
                        parames.Add(new MySqlParameter(properties.Item1, properties.Item2));
                    }
                }
            }
            return parames.ToArray();
        }
        #endregion
    }
}
