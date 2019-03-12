using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    /// <summary>
    /// 异步io控制器
    /// </summary>
    public class AsyncIoController: BaseController
    {
        private readonly IAsyncIoService _asyncIoService;
   
        public AsyncIoController(IAsyncIoService asyncIoService)
        {
            _asyncIoService = asyncIoService;
        }

        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public  BaseResponse SyncIoExcel()
        {
            return  _asyncIoService.SyncIoExcel();
        }

        /// <summary>
        /// 异步方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> AsyncIoExcel()
        {
            return await _asyncIoService.AsyncIoExcel();
        }
    }
}
