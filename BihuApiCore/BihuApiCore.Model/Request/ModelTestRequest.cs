using System.ComponentModel.DataAnnotations;

namespace BihuApiCore.Model.Request
{
    public class ModelTestRequest
    {
        /// <summary>
        ///     邀请码
        /// </summary>
        [Required]
        [RegularExpression(@"^[1-9][\d]{1,19}$", ErrorMessage = "邀请码错误，请重新输入！")]
        public string ShareCode { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        [Required]
        [RegularExpression(@"^1[3-9][0-9]{9}$", ErrorMessage = "手机号错误，请重新输入！")]
        public string Phone { get; set; }

        /// <summary>
        ///     验证码
        /// </summary>
        [Required]
        [RegularExpression(@"^[\d]{1,8}", ErrorMessage = "验证码错误，请重新输入！")]
        public string Code { get; set; }
    }
}
