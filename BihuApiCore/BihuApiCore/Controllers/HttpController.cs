using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;

namespace BihuApiCore.Controllers
{
    public class HttpController: BaseController
    {
        private readonly IHttpService _loginService;
       
        public HttpController(IHttpService loginService)
        {
            _loginService = loginService;
          
        }
        
        public async Task<BaseResponse> FormRequest()
        {						
            return await _loginService.FormRequest();
        }

    }
}
