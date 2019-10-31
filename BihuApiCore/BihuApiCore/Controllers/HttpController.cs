﻿using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BihuApiCore.Controllers
{
    /// <summary>
    /// http请求控制器
    /// </summary>
    public class HttpController: ApiBaseController
    {
        private readonly IHttpService _loginService;
       
        public HttpController(IHttpService loginService)
        {
            _loginService = loginService;          
        }

        /// <summary>
        /// 表单请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("FormRequest")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> FormRequest()
        {						
            return await _loginService.FormRequest();
        }

        /// <summary>
        /// 测试Authorize
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Test")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> Test()
        {						
            return await _loginService.FormRequest();
        }


        
    }
}
