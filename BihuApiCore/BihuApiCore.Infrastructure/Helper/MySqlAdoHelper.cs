using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace BihuApiCore.Infrastructure.Helper
{
    public sealed class MySqlAdoHelper
    {
        #region 构造函数及常量

        /// <summary>
        /// 批量操作每批次记录数
        /// </summary>
        public int BatchSize = 2000;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int CommandTimeOut = 600;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///初始化MySqlHelper实例
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public MySqlAdoHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region ExecuteNonQuery

        #region 非事务方法

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(ConnectionString, CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(ConnectionString, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                return ExecuteNonQuery(connection, null,commandType, commandText, parms);
            }
            catch (Exception ex)
            {
                throw new Exception($"MySqlAdoHelper执行sql:{commandText}", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        #endregion

        #region 事务方法

        public int ExecuteNonQueryTrans(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(transaction.Connection, transaction, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        private int ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlCommand command = new MySqlCommand();
            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            return command.ExecuteNonQuery();
            //command.Parameters.Clear();  不知道为啥要加这个 所以注释掉
        }

        #endregion

        #region 异步方法

        

        #endregion

        #endregion 

        #region ExecuteDataTable

        #region 非事务方法

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataTable(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(commandType, commandText, parms).Tables[0];
        }

        #endregion

        #region transaction version

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(transaction, commandType, commandText, parms).Tables[0];
        }

        #endregion

        #endregion 

        #region ExecuteDataSet

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public DataSet ExecuteDataSet( CommandType commandType, string commandText,params MySqlParameter[] parms)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                return ExecuteDataSet(connection, commandType, commandText, parms);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public  DataSet ExecuteDataSet(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(connection, null, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public  DataSet ExecuteDataSet(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(transaction.Connection, transaction, commandType, commandText, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        private DataSet ExecuteDataSet(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            var command = new MySqlCommand();
     
            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            var adapter = new MySqlDataAdapter(command);

            var ds = new DataSet();
            adapter.Fill(ds);
            //if (commandText.IndexOf("@") > 0)//不知道在干吗，所以注释掉
            //{
            //    commandText = commandText.ToLower();
            //    var index = commandText.IndexOf("where ");
            //    if (index < 0)
            //    {
            //        index = commandText.IndexOf("\nwhere");
            //    }
            //    ds.ExtendedProperties.Add("SQL", index > 0 ? commandText.Substring(0, index - 1) : commandText);
            //}
            //else
            //{
            //    ds.ExtendedProperties.Add("SQL", commandText);  //将获取的语句保存在表的一个附属数组里，方便更新时生成CommandBuilder
            //}

            foreach (DataTable dt in ds.Tables)
            {
                dt.ExtendedProperties.Add("SQL", ds.ExtendedProperties["SQL"]);
            }
            //command.Parameters.Clear(); //不知道在干吗，所以注释掉
            return ds;
        }

        #endregion ExecuteDataSet

        #region 辅助方法

        private void PrepareCommand(MySqlCommand command, MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] parms)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            command.Connection = connection;
            command.CommandTimeout = CommandTimeOut;
            // 设置命令文本(存储过程名或SQL语句)
            command.CommandText = commandText;
            // 分配事务
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            // 设置命令类型.
            command.CommandType = commandType;
            if (parms != null && parms.Length > 0)
            {
                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value;
                foreach (MySqlParameter parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))//是需要输入，且值为空
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
                command.Parameters.AddRange(parms);
            }
        }

        #endregion

        #region 批量插入

        /// <summary>
        ///大批量数据插入,返回成功插入行数（一般应用场景是从一个数据库查出一部分数据，然后插入到另一个数据库）
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns>返回成功插入行数</returns>
        public int BulkInsert(DataTable table)
        {
            return BulkInsert(ConnectionString, table);
        }

        /// <summary>
        ///大批量数据插入,返回成功插入行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="table">数据表</param>
        /// <returns>返回成功插入行数</returns>
        public static int BulkInsert(string connectionString, DataTable table)
        {
            if (string.IsNullOrEmpty(table.TableName)) throw new Exception("请给DataTable的TableName属性附上表名称");
            if (table.Rows.Count == 0) return 0;
            int insertCount;
            string tmpPath = Path.GetTempFileName();
            //string csv = DataTableToCsv(table);  这里要记录文件，我就不记录了
            //File.WriteAllText(tmpPath, csv);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
            
                MySqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                    {
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        EscapeCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = tmpPath,
                        NumberOfLinesToSkip = 0,
                        TableName = table.TableName,
                    };
                    bulk.Columns.AddRange(table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList());
                    insertCount = bulk.Load();
                  
                    tran.Commit();
                }
                catch (MySqlException)
                {
                    if (tran != null) tran.Rollback();
                    throw;
                }
            }
            //File.Delete(tmpPath);
            return insertCount;
        }

        ///// <summary>
        /////将DataTable转换为标准的CSV
        ///// </summary>
        ///// <param name="table">数据表</param>
        ///// <returns>返回标准的CSV</returns>
        //private static string DataTableToCsv(DataTable table)
        //{
        //    //以半角逗号（即,）作分隔符，列为空也要表达其存在。
        //    //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
        //    //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
        //    StringBuilder sb = new StringBuilder();
        //    foreach (DataRow row in table.Rows)
        //    {
        //        for (int i = 0; i < table.Columns.Count; i++)
        //        {
        //            var colum = table.Columns[i];
        //            if (i != 0) sb.Append(",");
        //            if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
        //            {
        //                sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
        //            }
        //            else sb.Append(row[colum]);
        //        }
        //        sb.AppendLine();
        //    }

        //    return sb.ToString();
        //}

        #endregion

    }
}
