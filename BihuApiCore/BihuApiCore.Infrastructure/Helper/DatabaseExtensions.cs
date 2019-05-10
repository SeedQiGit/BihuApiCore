using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
            where T : new()
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
            where T : new()
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

        /// <summary>
        /// 获取第一行数据（这么做相比于SqlQueryExt，唯一的好处就是节约内存了，如果是多行数据的情况下，list的多条内存，变为单条内存）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T SqlQuerySingle<T>(this DbContext db, string sql, params DbParameter[] parameters)
           where T : new()
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
            finally
            {
                conn.Close();
            }
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
        ///  执行原生语句相当于原生ExecuteScalar
        /// </summary>
        /// <param name="db"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
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
    }
}
