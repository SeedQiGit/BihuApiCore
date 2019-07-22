using System.Collections.Generic;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore.Models;
using Dapper;
using MySql.Data.MySqlClient;

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

            var list =(await _connection.QueryAsync<Product>("select * from product", new { UserName = "jack" })).ToList();

            return BaseResponse<List<Product>>.Ok(list);
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
