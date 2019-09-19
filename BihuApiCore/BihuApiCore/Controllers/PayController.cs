using BihuApiCore.Infrastructure.AliPay;
using BihuApiCore.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class PayController : BaseController
    {

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

    }
}
