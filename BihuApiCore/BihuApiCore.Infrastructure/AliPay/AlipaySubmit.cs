using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Infrastructure.AliPay
{
    /// <summary>
    /// 支付宝各接口请求提交类
    /// <remarks>构造支付宝各接口表单HTML文本，获取远程HTTP数据</remarks>
    /// </summary>
    public class AlipaySubmit
    {
        #region 字段

        //支付宝网关地址（新）
        private static string GATEWAY_NEW = "https://mapi.alipay.com/gateway.do?";
        //商户的私钥
        private static string _key;
        //编码格式
        private static string _input_charset;
        //签名方式
        private static string _sign_type;
        #endregion

        static AlipaySubmit()
        {
            _key = AlipayConfig.Key.Trim();
            _input_charset = AlipayConfig.Input_charset.Trim().ToLower();
            _sign_type = AlipayConfig.Sign_type.Trim().ToUpper();
        }

        /// <summary>
        /// 生成请求时的签名
        /// </summary>
        /// <param name="sPara">请求给支付宝的参数数组</param>
        /// <returns>签名结果</returns>
        private static string BuildRequestMysign(Dictionary<string, string> sPara)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string prestr = AlipayCore.CreateLinkString(sPara);

            //把最终的字符串签名，获得签名结果
            string mysign;
            switch (_sign_type)
            {
                case "MD5":
                    mysign = AlipayMd5.Sign(prestr, _key, _input_charset);
                    break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }

        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <returns>要请求的参数数组</returns>
        private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp)
        {
            //待签名请求参数数组
            //签名结果
            string mysign = "";

            //过滤签名参数数组
            var sPara = AlipayCore.FilterPara(sParaTemp);

            //获得签名结果
            mysign = BuildRequestMysign(sPara);

            //签名结果与签名方式加入请求提交参数组中
            sPara.Add("sign", mysign);
            sPara.Add("sign_type", _sign_type);

            return sPara;
        }

        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="code">字符编码</param>
        /// <returns>要请求的参数数组字符串</returns>
        private static string BuildRequestParaToString(SortedDictionary<string, string> sParaTemp, Encoding code)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            sPara = BuildRequestPara(sParaTemp);

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
            string strRequestData = AlipayCore.CreateLinkStringUrlencode(sPara, code);

            return strRequestData;
        }

        /// <summary>
        /// 建立请求，以表单HTML形式构造（默认）
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <param name="strMethod">提交方式。两个值可选：post、get</param>
        /// <param name="strButtonValue">确认按钮显示文字</param>
        /// <returns>提交表单HTML文本</returns>
        public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue)
        {
            //待请求参数数组
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            dicPara = BuildRequestPara(sParaTemp);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + GATEWAY_NEW + "_input_charset=" + _input_charset + "' method='" + strMethod.ToLower().Trim() + "'>");

            foreach (KeyValuePair<string, string> temp in dicPara)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='" + strButtonValue + "' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }

    }

    /// <summary>
    /// 支付宝支付，配置信息类
    /// </summary>
    public class AlipayConfig
    {
        #region 字段
        private static string _partner;
        private static string _key;
        private static string _input_charset;
        private static string _sign_type;
        #endregion

        static AlipayConfig()
        {
            _partner = "2088711591165758";
            _key = "klh1tbiusvvyzwrjt05a4i4lzx2igl59";
            _input_charset = "utf-8";
            //签名方式，选择项：RSA、DSA、MD5
            _sign_type = "MD5";
        }

        #region 属性

        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return _partner; }
            set { _partner = value; }
        }

        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public static string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return _input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return _sign_type; }
        }

        #endregion
    }
}
