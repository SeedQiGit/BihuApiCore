using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using BihuApiCore.Filters;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BihuApiCore.Controllers
{
    public class RegularExpressionController:BaseController
    {
        private readonly ILogger<RegularExpressionController> _logger;

        public RegularExpressionController( ILogger<RegularExpressionController> _logger )
        {
            _logger = _logger;
        }

        [HttpPost]
        //[ModelVerifyFilter] 这里用全局的
        public async Task<BaseResponse> TestPost([FromBody]RegularExpressionRequest request)
        {
            
            return BaseResponse.Ok();
        }



    }
}
