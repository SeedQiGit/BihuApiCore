using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BihuApiCore.Infrastructure.Helper.EF
{
    public static class EfSqlExtensions
    {

        #region EF查询数据，并转换为指定模型

        public static List<T> SqlQuery<T>(this DbContext db, string sql, params DbParameter[] parameters)
        {
            var dt = SqlQuery(db, sql, parameters);
            return dt.ToList<T>();
        }

        public static async Task<List<T>> SqlQueryAsync<T>(this DbContext db, string sql, params DbParameter[] parameters)
        {
            var dt = await SqlQueryAsync(db, sql, parameters);
            return dt.ToList<T>();
        }

        #region EF Core使用原生SQL读取为DataTable

        public static DataTable SqlQuery(this DbContext db, string sql, params DbParameter[] parameters)
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
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            finally
            {
                conn.Close();
            }
        }
        
        public static async Task<DataTable> SqlQueryAsync(this DbContext db, string sql, params DbParameter[] parameters)
        {
            //注意：不要对GetDbConnection获取到的conn进行using或者调用Dispose，否则DbContext后续不能再进行使用了，会抛异常
            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                using (var reader =await PreCommandReaderAsync(db.Database, sql, parameters))
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #endregion

        #region 语句预处理

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
            //where T : new()
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
                    Type type = typeof(T);
                    if (await reader.ReadAsync())
                    {
                        if (ObjectExtession.IsValueType(type))//增加对可空类型和string datetime兼容
                        {
                            return (T)ObjectExtession.DbChangeType(reader[0],type);
                        }
                        var propts = type.GetProperties();
                        T model = Activator.CreateInstance<T>();
                        foreach (var propt in propts)
                        {
                            object value = reader[propt.Name];
                            propt.SetValue(model, value == DBNull.Value ? null : value);
                        } 
                        return model;
                    }
                    else
                    {
                        throw new Exception("SqlQueryFirst未读取到任何数据");
                    }
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
            //where T : class,new()
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
                    T model = Activator.CreateInstance<T>();
                    var type = typeof(T);
                    var propts =type.GetProperties();
                    if (reader.Read())
                    {
                        if (ObjectExtession.IsValueType(type))//增加对可空类型和string datetime兼容
                        {
                            return (T)ObjectExtession.DbChangeType(reader[0],type);
                        }
                        else
                        {
                            foreach (var propt in propts)
                            {
                                object value = reader[propt.Name];
                                propt.SetValue(model, value == DBNull.Value ? null : value);
                            } 
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

        #region ExecuteScalar  直接使用  Context.Database.ExecuteSqlCommand(sql);

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

        #region 很久之前的方法查询方法，已经弃用，这个是while (reader.Read()) 然后直接循环属性赋值

        public static async Task<List<T>> SqlQueryAsyncOri<T>(this DbContext db, string sql, params DbParameter[] parameters)
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

    }
}
