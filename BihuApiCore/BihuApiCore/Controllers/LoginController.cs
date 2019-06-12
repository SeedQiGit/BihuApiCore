using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;

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

    }
}
