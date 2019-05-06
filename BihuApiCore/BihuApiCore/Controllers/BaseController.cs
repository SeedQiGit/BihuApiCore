using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    //[Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        ///     返回成功结果
        /// </summary>
        /// <param name="message">结果信息</param>
        /// <returns></returns>
        protected virtual BaseResponse Success(string message = null)
        {
            return BaseResponse.Ok(message);
        }

        /// <summary>
        ///     返回成功结果
        /// </summary>
        /// <param name="data">结果数据集</param>
        /// <returns></returns>
        public virtual BaseResponse<T> Success<T>(T data)
        {
            return BaseResponse<T>.Ok(data);
        }

    }
}