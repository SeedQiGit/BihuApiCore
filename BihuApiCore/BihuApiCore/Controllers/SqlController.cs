using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class SqlController:BaseController
    {
        private readonly ISqlService _sqlService;
        private readonly IDapperService _dapperService;

        public SqlController(ISqlService sqlService, IDapperService dapperService)
        {
            _sqlService = sqlService;
            _dapperService = dapperService;
        }

        #region 获取列表，带分页

        [HttpGet]
        public async Task<BaseResponse<PageData<User>>> GetUserList([FromQuery]PageRequest request)
        {
            return SetStatusCode(await _sqlService.GetUserList(request));
        }

        #endregion
        
        #region 测试事务

        [HttpGet]
        public async Task<BaseResponse> TestTransaction()
        {
            return SetStatusCode(await _sqlService.TestTransaction());
        }

        #endregion

        #region 测试EF

        [HttpGet]
        public async Task<BaseResponse> TestSql()
        {
            return SetStatusCode(await _sqlService.TestSql());
        }

        [HttpGet]
        public async Task<BaseResponse> ExecuteSqlCommandAsync()
        {
            return SetStatusCode(await _sqlService.ExecuteSqlCommandAsync());
        }

        [HttpGet]
        public async Task<BaseResponse> TestCompareValueAndassign()
        {
            return SetStatusCode(await _sqlService.TestCompareValueAndassign());
        }


        #endregion

        #region Dapper相关

        
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> DapperGetList()
        {
            return SetStatusCode(await _dapperService.DapperGetList());
        }



        /// <summary>
        /// 批量插入
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> DapperBulkInsert()
        {
            return SetStatusCode(await _dapperService.DapperBulkInsert());
        }

        #endregion


    }
}
