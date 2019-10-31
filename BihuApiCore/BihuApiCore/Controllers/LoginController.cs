using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;
using BihuApiCore.Model.Request;

namespace BihuApiCore.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ILoginService _loginService;
       
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
          
        }
        
        public async Task<BaseResponse> Add()
        {						
            await _loginService.Add();
            return Ok();
        }

        public async Task<BaseResponse> ModelTest(ModelTestRequest request)
        {						
            
            return Ok();
        }
    }
}
