using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace BihuApiCore.Model.Request
{
    public class BaseRequest
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Required,Range(1, Int64.MaxValue, ErrorMessage = "UserID超出范围")]
        public Int64 UserId { get; set; }
    }
    public static class BaseRequestExtension
    {
        /// <summary>
        /// 根据claims获取请求基类参数
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="request"></param>
        public static void GetBaseRequest(this IEnumerable<Claim>  claims,BaseRequest request)
        {
            var claimList = claims.ToList();
            var employeeIdClaim =claimList.FirstOrDefault(c => c.Type == "UserId");
            if (employeeIdClaim==null)
            { 
                throw new Exception("claim信息有误");
            }
            request.UserId = Convert.ToInt64(employeeIdClaim.Value);
        }
    }
}
