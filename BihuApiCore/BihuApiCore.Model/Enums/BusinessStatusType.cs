using System.ComponentModel;

namespace BihuApiCore.Model.Enums
{
    /// <summary>
    /// 业务状态码
    /// </summary>
    public enum BusinessStatusType
    {
        [Description("失败")]
        Failed = 0,

        [Description("成功")]
        OK = 1,

        [Description("请确认操作")]
        Confirm = 2,

        [Description("用户未登录或登录到期")]
        LoginExpire = 9,

        [Description("服务端拒绝访问")]
        Forbidden = 403,

        [Description("未找到对应内容")]
        NotFindObject = 404,

        [Description("没有权限")]
        NoAuthority = 405,

        [Description("必填项为空")]
        ItemEmpty = 406,

        [Description("请求过于频繁")]
        FrequencyRequest = 429,

        [Description("参数错误")]
        ParameterError = -10000,
   
        [Description("请求发生异常")]
        Error = -10003
    }
}
