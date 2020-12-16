using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using Dnc.Api.Throttle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
//#if !DEBUG
//      [Authorize]
//#endif
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
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

        #region 根据code生成Http状态码

        protected BaseResponse SetStatusCode(BaseResponse response)
        {
            JudgeStatus(response.Code);
            return response;
        }

        protected BaseResponse<T> SetStatusCode<T>(BaseResponse<T> response)
        {
            JudgeStatus(response.Code);
            return response;
        }

        private void JudgeStatus(int code)
        {
            switch (code)
            {
                case (int)BusinessStatusType.OK:
                    HttpContext.Response.StatusCode = 200; break;
                case 401:
                    HttpContext.Response.StatusCode = 401; break;
                default: 
                    HttpContext.Response.StatusCode = 400;
                    break;
            }
        }

        #endregion
    }
}