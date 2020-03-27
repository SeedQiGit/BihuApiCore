using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BihuApiCore.Events.Event
{


    public class CBSQuoteCallBackIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 智能任务Id guid
        /// </summary>
        public string TaskId { get; set; }

        public long Agent { get; set; }

        public long ChildAgent { get; set; }

        public long ReQuoteAgent { get; set; }

        public string ReQuoteName { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { set; get; }

        /// <summary>
        /// 报价保险公司
        /// </summary>
        public int QuoteCompany { get; set; }


        /// <summary>
        /// 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }


        public string Guid { get; set; }


        /// <summary>
        /// 报价时间
        /// </summary>
        public string QuoteTime { get; set; }


        /// <summary>
        /// 先定义成string，用于排查不确定的问题
        /// </summary>
        public string  Buid { get; set; }


        /// <summary>
        ///-1 正在核保中  0 核保失败  1核保成功  2 未到期未核保 3人工审核中  4 未勾选核保 5 报价失败未核保  6核保功能关闭 
        /// </summary>
        public int SubmitStatus { get; set; }



        /// <summary>
        /// 核保结果描述
        /// </summary>
        public string SubmitResult { get; set; }
       


    }
    public class IntegrationEvent
    {
        public Guid Id { get; }
        
        public DateTime CreationDate { get; }
    }
}
