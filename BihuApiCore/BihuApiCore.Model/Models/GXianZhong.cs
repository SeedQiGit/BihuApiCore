using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Model.Models
{
    public class GXianZhong
    {
        public XianZhongUnit SanZheJieJiaRi { get; set; }
        public XianZhongUnit CheSun { get; set; }
        public XianZhongUnit SanZhe { get; set; }
        public XianZhongUnit DaoQiang { get; set; }
        public XianZhongUnit SiJi { get; set; }
        public XianZhongUnit ChengKe { get; set; }
        /// <summary>
        /// 2进口1国产0不投保
        /// </summary>
        public XianZhongUnit BoLi { get; set; }
        public XianZhongUnit HuaHen { get; set; }
        public XianZhongUnit SheShui { get; set; }
        public XianZhongUnit ZiRan { get; set; }
        public XianZhongUnit TeYue { get; set; }
        public XianZhongUnit BuJiMianCheSun { get; set; }
        public XianZhongUnit BuJiMianSanZhe { get; set; }
        public XianZhongUnit BuJiMianDaoQiang { get; set; }

    }
    public class XianZhongUnit
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
}
