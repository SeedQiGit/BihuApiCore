using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Model.Models
{
    public class XianZhongBase
    {
        /// <summary>
        /// 保额
        /// </summary>
        public double baoE { get; set; }
        /// <summary>
        /// 保费
        /// </summary>
        public double baoFei { get; set; }
        /// <summary>
        /// 不计免赔
        /// </summary>
        public double buJiMianBaoFei { get; set; }
        /// <summary>
        /// 不计
        /// </summary>
        public double buJiMian { get; set; }
    }

    public class XianZhongF
    {
        /// <summary>
        /// 
        /// </summary>
        public CheSun cheSun { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SanZhe sanZhe { get; set; }

        /// <summary>
        /// 不计免附加总额
        /// </summary>
        public double BuJiMianFuJiaTotalAmount { get; set; }

   
      
        /// <summary>
        /// 
        /// </summary>
        public SiJi siJi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChengKe chengKe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DaoQiang daoQiang { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HuaHen huaHen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BoLi boLi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZiRan ziRan { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SheShui sheShui { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SanFangTeYue sanFangTeYue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XiuLiChang xiuLiChang { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SheBei sheBei { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SanZheJieJiaRi SanZheJieJiaRi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XiuLiBuChang XiuLiBuChang { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double forceTotal { get; set; }

        public double taxTotal { get; set; }
        /// <summary>
        /// 商业险合计
        /// </summary>
        public double bizTotal { get; set; }
    }

   

    public class CheSun : XianZhongBase
    {


    }

    public class SanZhe : XianZhongBase
    {

    }

    public class SiJi : XianZhongBase
    {

    }

    public class ChengKe : XianZhongBase
    {

    }

    public class DaoQiang : XianZhongBase
    {

    }

    public class HuaHen : XianZhongBase
    {

    }

    public class BoLi : XianZhongBase
    {

    }

    public class ZiRan : XianZhongBase
    {

    }

    public class SheShui : XianZhongBase
    {

    }

    public class SanFangTeYue : XianZhongBase
    {
    }

    public class XiuLiChang : XianZhongBase
    {
        /// <summary>
        /// 修理厂系数
        /// </summary>
        public double? xiLiChangNumber { get; set; }
    }

    public class SheBei : XianZhongBase
    {

    }

    public class SanZheJieJiaRi : XianZhongBase
    {

    }

    public class XiuLiBuChang : XianZhongBase
    {

        /// <summary>
        /// 
        /// </summary>
        public int days { get; set; }

        public double XiShu { get; set; }
    }




}
