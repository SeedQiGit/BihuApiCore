using System.Collections.Generic;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Infrastructure.Helper;
using MySql.Data.MySqlClient;

namespace BihuApiCore.Service.Implementations
{
    public class AdoService:IAdoService
    {
        private readonly MySqlAdoHelper _mySqlAdoHelper=new MySqlAdoHelper(ConfigurationManager.GetValue<string>("ConnectionStrings:EntityContext"));

        public  AdoService()
        {

        }

        #region SqlServer

        public async Task<BaseResponse> SqlServerCommand()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.GetValue<string>("ConnectionStrings:SqlServer"));
            if (conn.State!=ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
          
            using(SqlCommand cmd = new SqlCommand())//创建命令对象SqlCommand
            {
                cmd.Connection = conn;//设置连接对象
                cmd.CommandType = CommandType.Text;//设置Command对象执行语句的类型
                cmd.CommandText = "Delete from Car where Title='Benz';";//设置执行的语句
                return BaseResponse.Ok((await cmd.ExecuteNonQueryAsync()).ToString());
            }
        }

        public async Task<BaseResponse> SqlServerDataReader()
        {
            var dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.GetValue<string>("ConnectionStrings:SqlServer")))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from Car;";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return BaseResponse<DataTable>.Ok(dt);
        }

        #endregion

        #region mysql

        public async Task<BaseResponse> MysqlCommand()
        {
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.GetValue<string>("ConnectionStrings:EntityContext")))
            {
                if (conn.State!=ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
          
                using(MySqlCommand cmd = new MySqlCommand())//创建命令对象SqlCommand
                {
                    cmd.Connection = conn;//设置连接对象
                    cmd.CommandType = CommandType.Text;//设置Command对象执行语句的类型
                    cmd.CommandText = "Delete from zs_picc_call where id=89;";//设置执行的语句
                    return BaseResponse.Ok((await cmd.ExecuteNonQueryAsync()).ToString());
                } 
            }
        }

        public async Task<BaseResponse> MysqlDataReader()
        {
            var dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.GetValue<string>("ConnectionStrings:EntityContext")))
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from zs_picc_call;";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return BaseResponse<DataTable>.Ok(dt);
        }

        #endregion

        #region MySql Components

        #region ExecuteNonQuery

        public async Task<BaseResponse> MysqlExecuteNonQuery()
        {
            var sql = @"INSERT INTO bihu_apicore_test.product (Name,Price,Description)VALUES(?Name,?Price,?Description)";
            List<MySqlParameter> ps = new List<MySqlParameter>() {
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "Name" , Value= "一号产品" },
                new MySqlParameter {  MySqlDbType=MySqlDbType.Decimal,ParameterName= "Price" , Value= 12.222},
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "Description" , Value= "无敌是多么寂寞"}
            };
            //_mySqlAdoHelper.ExecuteNonQuery( sql, ps.ToArray());
            return BaseResponse<string>.Ok(_mySqlAdoHelper.ExecuteNonQuery( sql, ps.ToArray()).ToString());
        }

        #endregion

        #endregion

    }
}
