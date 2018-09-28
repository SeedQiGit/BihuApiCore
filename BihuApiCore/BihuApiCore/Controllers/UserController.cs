﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Filters;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Dto;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BihuApiCore.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        /// <summary>
        /// 测试get 同步
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Test")]
        [ProducesResponseType(typeof(UserDto), 1)]
        public BaseResponse Test()
        {
            return _userService.Test();
        }
        /// <summary>
        /// 测试get 异步
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("TestAsy")]
        public async Task<BaseResponse> TestAsy()
        {
            return await _userService.TestAsy();
            //return await Task.Run(()=> { return BaseResponse.GetBaseResponse(BusinessStatusType.OK); });
        }

        [HttpPost("TestPost")]
        [ModelVerifyFilter]
        public async Task<BaseResponse> TestPost([FromBody] BaseRequest request)
        {
            return await Task.Run(()=> { return BaseResponse.GetBaseResponse(BusinessStatusType.Ok); });
        }


    }
}