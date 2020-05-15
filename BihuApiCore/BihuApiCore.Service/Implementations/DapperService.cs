using System.Collections.Generic;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;

namespace BihuApiCore.Service.Implementations
{
    public class DapperService:IDapperService
    {
        private readonly IDbConnection _connection;

        public DapperService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<BaseResponse> DapperGetList()
        {

            // var list =(await _connection.QueryAsync<Product>("select * from product", new { UserName = "jack" })).ToList();
            var sql = "SELECT * FROM `company_module_relation` where Updatedtime between ?StratDataTime and ?EndDataTime";

            var StratDataTime = DateTime.Now.AddYears(-1);
            var EndDataTime = DateTime.Now.AddYears(1);
            List<MySqlParameter> parameters = new List<MySqlParameter>() {
                new MySqlParameter {  MySqlDbType=MySqlDbType.Date,ParameterName= "StratDataTime" ,
                    Value= StratDataTime},
                new MySqlParameter {  MySqlDbType=MySqlDbType.Date,ParameterName= "EndDataTime" ,
                    Value=EndDataTime},
            };
            var args = new DynamicParameters(new { });
            parameters.ForEach(p => args.Add(p.ParameterName, p.Value));

            //声明动态参数
            DynamicParameters Parameters = new DynamicParameters();
            Parameters.Add("StratDataTime", StratDataTime,DbType.DateTime);
            Parameters.Add("EndDataTime", EndDataTime,DbType.DateTime);

            var list = (await _connection.QueryAsync<CompanyModuleRelation>(sql, Parameters)).ToList();


            return BaseResponse<List<CompanyModuleRelation>>.Ok(list);
        }

        public async Task<BaseResponse> DapperBulkInsert()
        {
            var sql = @"INSERT INTO product (Name,Price,Description)VALUES(?Name,?Price,?Description)";
            List<Product>  list=new List<Product>
            {
                new Product{  Name="1",Price  = 1,Description = "111"},
                new Product{  Name="2",Price  = 1,Description = "111"},
                new Product{  Name="2",Price  = 1,Description = "111"},
            };

            var result =(await _connection.ExecuteAsync(sql,list)).ToString();
            return BaseResponse.Ok(result);
        }
    }
}
