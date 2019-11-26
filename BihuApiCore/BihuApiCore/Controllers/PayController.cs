using BihuApiCore.Infrastructure.AliPay;
using BihuApiCore.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BihuApiCore.Controllers
{
    public class PayController : BaseController
    {
        private readonly ILogger<PayController> _logger;

        public PayController( ILogger<PayController> logger)
        {
            _logger = logger;
        }

        #region 短信支付跳转页面

        /// <summary>
        /// 短信支付跳转页面   所有充值都是给自己充值，避免我充值，其他人消费了
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AliPay()
        {
            string payment_type = "1";
            //服务器异步通知页面路径
            string notify_url = ConfigurationManager.GetValue("CarBusinessUrl") + "/CallBack/AsynUrl";
            //页面跳转同步通知页面路径
            string return_url = ConfigurationManager.GetValue("CarBusinessUrl") + "/Alipay/MessagePayReturnUrl";
            string seller_email = "Biz@91bihu.com";
            //商户订单号
            string out_trade_no = "2019091002111158";
            //订单名称
            string subject = "短信充值";
            //支付宝回传参数  
            //公用回传参数，如果请求时传递了该参数，则返回给商户时会回传该参数。支付宝只会在同步返回（包括跳转回商户网站）和异步通知时将该参数原样返回。本参数必须进行UrlEncode之后才可以发送给支付宝。
            //不过好像不好用，最后我还是用body了。。。。
            string passback_params= "用户id1231";
            //必填 
            string total_fee = "0.01";
            //订单描述       
            string body = "短信充值";
            //商品展示地址
            string show_url = "";
            //防钓鱼时间戳
            string anti_phishing_key = "";
            //客户端的IP地址
            string exter_invoke_ip = "";
            string it_b_pay = "20m";

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", AlipayConfig.Partner);
            sParaTemp.Add("_input_charset", AlipayConfig.Input_charset.ToLower());
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("it_b_pay", it_b_pay);//订单未支付超时时间
            sParaTemp.Add("seller_email", seller_email);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("passback_params", passback_params);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("anti_phishing_key", anti_phishing_key);
            sParaTemp.Add("exter_invoke_ip", exter_invoke_ip);

            //建立请求
            string sHtmlText = AlipaySubmit.BuildRequest(sParaTemp, "get", "确认");

            Response.ContentType = "text/html";

            //不加utf-8，输出会乱码
            StreamWriter sw = new StreamWriter(Response.Body, Encoding.UTF8);
            await sw.WriteAsync(sHtmlText);
            await sw.FlushAsync();

            return new EmptyResult();//也可以直接用task
        }

        #endregion
           #region 短信支付后回调地址

        /// <summary>
        /// 短信支付后回调地址 ReturnUrl
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> MessagePayReturnUrl()
        {
            _logger.LogInformation("接收到短信支付后回调;" );
            SortedDictionary<string, string> sPara = GetRequestGet();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Query["notify_id"], Request.Query["sign"]);
                string content = string.Format("交易号：{0}^订单号：{1}^交易状态：{2}^交易金额：{3}", Request.Query["trade_no"], Request.Query["out_trade_no"], Request.Query["trade_status"], Request.Query["total_fee"]);
                _logger.LogInformation("同步回调：" + content);
                if (verifyResult)//验证成功
                {
                    //订单号
                    string out_trade_no = Request.Query["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.Query["trade_no"];

                    //交易状态
                    string trade_status = Request.Query["trade_status"];

                    string subject= Request.Query["body"];
                
                    if (Request.Query["trade_status"] == "TRADE_FINISHED" || Request.Query["trade_status"] == "TRADE_SUCCESS")
                    {

                        #region 调用支付成功服务  是不是在通知里面调用会更好？？   都调用以下吧，做成幂等的 加锁  但是影响反应速度啊！！！

#pragma warning disable 4014
                        
#pragma warning restore 4014
                        #endregion

                        return Redirect(ConfigurationManager.GetValue("ForeEndUrl")+"/#/marketManage/PaySuccess?orderNum="+out_trade_no);
                    }
                }
            }
            return Redirect(ConfigurationManager.GetValue("ForeEndUrl")+"/#/marketManage/PayFail");
          
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestGet()
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            IQueryCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Query;
           
            foreach (var item in coll)
            {
                sArray.Add(item.Key, item.Value);
            }

            return sArray;
        }

        #endregion

        #region 短信支付后阿里通知地址

        /// <summary>
        ///  短信支付后阿里通知地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> MessagePayNotify()
        {
            _logger.LogInformation("收到短信支付后通知" );
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);
                string content = string.Format("交易号:{0}^订单号:{1}^交易状态:{2}^交易金额:{3}", Request.Form["trade_no"], Request.Form["out_trade_no"], Request.Form["trade_status"], Request.Form["total_fee"]);
                _logger.LogInformation("异步回调：" + content);
                if (verifyResult)//验证成功
                {
                    //商户订单号
                    string out_trade_no = Request.Form["out_trade_no"];

                    //支付宝交易号
                    string trade_no = Request.Form["trade_no"];

                    string subject= Request.Query["body"];

                    //交易状态
                    string trade_status = Request.Form["trade_status"];
                    if (Request.Form["trade_status"] == "TRADE_SUCCESS" || Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。

#pragma warning disable 4014
                        
#pragma warning restore 4014
                    }
                    return BaseResponse.Ok();
                   
                }
                else//验证失败
                {
                    return BaseResponse.Failed();
                }
            }
            return BaseResponse.Failed("无通知参数");
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost()
        {
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
       
            var coll = Request.Form;
           
            foreach (var item in coll)
            {
                sArray.Add(item.Key, item.Value);
            }

            return sArray;
        }

        #endregion





    }
}
