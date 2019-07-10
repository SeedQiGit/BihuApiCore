using System;

namespace BihuApiCore.Model.Enums
{
    /// <summary>
    /// H5移动端展示权限
    /// </summary>
    [Flags]
    public enum BxShopFlagsEnum

    {
        无=0,// 0000 0000  0
        非车商城h5 = 1 << 0,//  0000 0001  1
        显示车险 = 1 << 1, //   0000 0010  2
        显示微信=1 << 2,  //  0000 0100  4
    }
}
