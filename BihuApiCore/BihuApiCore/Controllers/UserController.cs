﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BihuApiCore.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public BaseResponse Test()
        {
            return _userService.Test();
        }

        [HttpGet]
        public async Task<BaseResponse> TestAsy()
        {
            return await _userService.TestAsy();
            //return await Task.Run(()=> { return BaseResponse.GetBaseResponse(BusinessStatusType.OK); });
        }

        //[HttpGet]
        //public async Task<BaseResponse> FriendView([FromBody] FriendRequest request)
        //{
        //    return await _friendService.FriendView(request);
        //}


    }
}