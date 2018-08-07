using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BihuApiCore.Model.Request
{
    public class BaseRequest
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Required,Range(1, Int64.MaxValue, ErrorMessage = "UserID超出范围")] //刘振龙6月9号注释 前端需要联调
        public Int64 UserId { get; set; }
    }
}
