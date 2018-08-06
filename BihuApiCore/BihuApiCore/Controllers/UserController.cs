using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    public class UserController : BaseController
    {


        [HttpGet]
        public BaseResponse Test()
        {

            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
        }

        [HttpGet]
        public async Task<BaseResponse> TestAsy()
        {
            return await Task.Run(()=> { return BaseResponse.GetBaseResponse(BusinessStatusType.OK); });
                
        }

        //[HttpGet]
        //public async Task<BaseResponse> FriendView([FromBody] FriendRequest request)
        //{
        //    return await _friendService.FriendView(request);
        //}


    }
}