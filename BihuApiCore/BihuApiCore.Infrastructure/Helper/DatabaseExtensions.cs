using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Helper
{
    public static class DatabaseExtensions
    {
        #region 查询 T : new类型(有公共无参的构造函数)

        /// <summary>
        ///  EF Core使用原生SQL的扩展方法  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> SqlQuery<T>(this DbContext db, string sql, params DbParameter[] parameters)
            where T : class,new()
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var reader = PreCommandReader(db.Database, sql, parameters))
                {
                    var propts = typeof(T).GetProperties();
                    var list = new List<T>();
                    T model;
                    object val;
                    while (reader.Read())
                    {
                        model = new T();
                        foreach (var l in propts)
                        {
                            val = reader[l.Name];
                            l.SetValue(model, val == DBNull.Value ? null : val);
                        }
                        list.Add(model);
                    }
                    return list;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        ///  EF Core使用原生SQL的扩展方法  异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<List<T>> SqlQueryAsync<T>(this DbContext db, string sql, params DbParameter[] parameters)
            where T : class,new()
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                using (var reader = await PreCommandReaderAsync(db.Database, sql, parameters))
                {
                    var propts = typeof(T).GetProperties();
                    var list = new List<T>();
                    T model;
                    object val;
                    while (await reader.ReadAsync())
                    {
                        model = new T();
                        foreach (var l in propts)
                        {
                            val = reader[l.Name];
                            l.SetValue(model, val == DBNull.Value ? null : val);
                        }
                        list.Add(model);
                    }
                    return list;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 判断值类型,string,datetime
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        private static bool IsValueType(Type tp)
        {
            return tp.IsValueType || tp == typeof(Nullable<>) || tp == typeof(string) || tp == typeof(DateTime);
        }

        private static DbDataReader PreCommandReader(DatabaseFacade context, string query, params DbParameter[] parameters)
        {
            using (var command = context.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.AddRange(parameters);
                command.CommandType = CommandType.Text;
                return command.ExecuteReader();
            }
        }

        private static async Task<DbDataReader> PreCommandReaderAsync(DatabaseFacade context, string query, params DbParameter[] parameters)
        {
            using (var command = context.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.AddRange(parameters);
                command.CommandType = CommandType.Text;
                return await command.ExecuteReaderAsync();
            }
        }

        #endregion

        #region Func转换类型 sql查询方法  

        public static async Task<List<T>> SqlFuncAsync<T>(this DbContext db, string sql, Func<DbDataReader, T> map, params DbParameter[] parameters)
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                using (var reader = await PreCommandReaderAsync(db.Database, sql, parameters))
                {
                    var list = new List<T>();
                    while (await reader.ReadAsync())
                    {
                        list.Add(map(reader));
                    }
                    return list;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("PreCommandReader执行sql:{0}", sql), ex);
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 获取单行数据  类似First()  未找到抛出异常

        /// <summary>
        /// 获取第一行数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<T> SqlQueryFirstAsync<T>(this DbContext db, string sql, params DbParameter[] parameters)
            where T : new()
        {
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                using (var reader = await PreCommandReaderAsync(db.Database, sql, parameters))
                {
                    T model = new T();
                    Type tp = typeof(T);
                  
                    if (await reader.ReadAsync())
                    {
                        if (tp.IsValueType)
                        {
                            return (T) Convert.ChangeType(reader.GetValue(0), tp);
                        }
                        var propts = tp.GetProperties();
                        foreach (var propt in propts)
                        {
                            object value = reader[propt.Name];
                            propt.SetValue(model, value == DBNull.Value ? null : value);
                        } 
                    }
                    else
                    {
                        throw new Exception("SqlQueryFirst未读取到任何数据");
                    }
                    
                    return model;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 获取第一行数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T SqlQueryFirst<T>(this DbContext db, string sql, params DbParameter[] parameters)
            where T : class,new()
        {
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var reader = PreCommandReader(db.Database, sql, parameters))
                {
                    T model = new T();
                    var propts = typeof(T).GetProperties();
                    if (reader.Read())
                    {
                        foreach (var propt in propts)
                        {
                            object value = reader[propt.Name];
                            propt.SetValue(model, value == DBNull.Value ? null : value);
                        } 
                    }
                    else
                    {
                        throw new Exception("SqlQueryFirst未读取到任何数据");
                    }
                    
                    return model;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 执行原生语句相当于原生ExecuteScalar

        public static async Task<object> ExecuteScalarAsync(this DbContext db, string query, params object[] parameters)
        {
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.Text;
                    return await command.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行ExecuteScalar语句出现异常", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        
        public static object ExecuteScalar(this DbContext db, string query, params object[] parameters)
        {
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var command = db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.Text;
                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行ExecuteScalar语句出现异常", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion
    }
}
