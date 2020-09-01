using BihuApiCore.Model.Models;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class ReflexController : BaseController
    {
        private readonly ILogger<ReflexController> _logger;
        private readonly IReflexService _reflexService;

        public ReflexController(ILogger<ReflexController> logger,IReflexService reflexService)
        {
            _logger = logger;
            _reflexService = reflexService;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> Test()
        {

            return BaseResponse.Ok();
        }


        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> XianZhongF([FromBody]XianZhongF request)
        {
            var res = await _reflexService.XianZhongF(request);
            return res;
            return BaseResponse.Ok();
        }
    }
}
