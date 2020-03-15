using System.ComponentModel.DataAnnotations;

namespace BihuApiCore.Model.Request
{
    public class RegularExpressionRequest
    {
        [RegularExpression(@"^1[^012]\d{9}$", ErrorMessage = "请输入正确的手机！")]
        public string Mobile { get; set; }
    }
}
