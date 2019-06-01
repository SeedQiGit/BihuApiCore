using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BihuApiCore.Controllers
{
    public class SqlController:BaseController
    {
        private readonly ISqlService _sqlService;
        private readonly ILogger<UserController> _logger;

        public SqlController(ILogger<UserController> logger,ISqlService sqlService)
        {
            _sqlService = sqlService;
            _logger = logger;
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

        #region 测试

        [HttpGet]
        public async Task<BaseResponse> TestSql()
        {
            return SetStatusCode(await _sqlService.TestSql());
        }

        #endregion

    }
}
