using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    /// <summary>
    /// Api基础控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController: ControllerBase
    {
        /// <summary>
        ///   返回成功结果
        /// </summary>
        /// <param name="message">结果信息</param>
        /// <returns></returns>
        protected virtual BaseResponse Ok(string message = null)
        {
            return BaseResponse.Ok(message);
        }

        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <param name="data">结果数据集</param>
        /// <returns></returns>
        protected virtual BaseResponse<T> Ok<T>(T data)
        {
            return BaseResponse<T>.Ok(data);
        }
    }
}
