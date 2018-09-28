using System;
using System.ComponentModel.DataAnnotations;

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
}
