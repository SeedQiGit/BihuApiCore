using System.Runtime.Serialization;

namespace BihuApiCore.Model.Response
{
    public class OrderDetailResponse
    {
        public OrderDetailResponse()
        {
            OrderDeliveryInfo = new OrderDeliveryInfo();
           
        }

        /// <summary>
        /// 配送信息
        /// </summary>
        public OrderDeliveryInfo OrderDeliveryInfo { get; set; }

      
    }

    public class OrderDeliveryInfo
    {

        /// <summary>
        /// 配送方式描述
        /// </summary>
        public string DeliveryMethodStr
        {
            get
            {
                return ((DeliveryMethodEnum)DeliveryMethod).ToString();
            }
        }

        /// <summary>
        /// -1默认值0快递保单1网点派送2网点自提
        /// </summary>
        [IgnoreDataMember]
        public int DeliveryMethod { get; set; }
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// 配送联系人
        /// </summary>
        public string DeliveryContacts { get; set; }
        /// <summary>
        /// 配送联系人电话
        /// </summary>
        public string DeliveryContactsMobile { get; set; }
        ///// <summary>
        ///// 待配送=0,已配送=1,
        ///// </summary>
        //public int DeliveryState { get; set; }

    }
    public enum DeliveryMethodEnum
    {
        快递保单=0,
        网点派送=1,
        网点自提=2,
        无=-1
    }
}
