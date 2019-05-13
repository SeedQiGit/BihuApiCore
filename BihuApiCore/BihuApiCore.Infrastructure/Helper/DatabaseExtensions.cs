using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
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

        #region 使用DataTable的查询方法

        public static List<T> SqlQueryDt<T>(this DbContext db, string sql, params DbParameter[] parameters)
        {
            var dt = SqlQuery(db, sql, parameters);
            return dt.ToList<T>();
        }

        public static async Task<List<T>> SqlQueryDtAsync<T>(this DbContext db, string sql, params DbParameter[] parameters)
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

        #region 辅助方法

        #region 类型判断方法

        /// <summary>
        /// 判断值类型,string,datetime
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        private static bool IsValueType(Type tp)
        {
            return tp.IsValueType || tp == typeof(Nullable<>) || tp == typeof(string) || tp == typeof(DateTime);
        }

        /// <summary>
        /// 空值判断 使用DbDataReader时候需要用这个判断
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDbNull<T>(T obj)
        {
            return obj == null || obj is DBNull;
        }

        #endregion

        #region 数据转换辅助方法
        
        /// <summary>
        /// 对可空类型进行判断转换，要不然会报错
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object DbChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (IsNullOrDbNull(value))
                    return null;
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 对可空类型进行判断转换，要不然会报错
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T DbChangeType<T>(object value)
        {
            Type conversionType = typeof(T);
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))//判断是否为可空类型
            {
                if (IsNullOrDbNull(value))
                    return default(T);
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return (T) Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 数据表转为列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            var list = new List<T>();
            //字段小写列表
            var fields = new List<string>();
            foreach (DataColumn dtColumn in dt.Columns)
            {
                fields.Add(dtColumn.ColumnName.ToLower());
            }
            //值类型直接返回第一列
            Type tp = typeof(T);
            if (IsValueType(tp))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(DbChangeType<T>(row[0]));
                }

                return list;
            }

            //属性列表
            var properties = tp.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            foreach (DataRow row in dt.Rows)
            {
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in properties)
                {
                    string field = property.Name;
                    //忽略字段大小写
                    if (!fields.Contains(field.ToLower())) continue;
                    //忽略空值,忽略只读属性
                    if (!IsNullOrDbNull(row[field]) && property.CanWrite)
                    {
                        property.SetValue(model, DbChangeType(row[field], property.PropertyType), null);
                    }
                }
                list.Add(model);
            }

            return list;
        }

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

        #region MyRegion

        



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
