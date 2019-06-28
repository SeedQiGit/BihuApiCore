using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Model.Response;

namespace BihuApiCore.Controllers
{
    public class ConfigController : BaseController
    {

        public ConfigController()
        {


        }

        public async Task<BaseResponse> Test()
        {
            var hostUrl = ConfigurationManager.GetValue("HostUrl");
            return Ok(hostUrl);
        }

    }
}
