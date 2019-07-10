using BihuApiCore.Model.Enums;
using System;
using Xunit;
using Xunit.Abstractions;

namespace BihuApiCore.Model.UnitTest
{
    public class BxShopFlagsEnumTest
    {
        protected readonly ITestOutputHelper Output;

        public BxShopFlagsEnumTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
        }
        [Fact]
        public void FlagsEnumTest()
        {
            BxShopFlagsEnum flags = BxShopFlagsEnum.显示微信 | BxShopFlagsEnum.显示车险 | BxShopFlagsEnum.非车商城h5;
            Output.WriteLine(flags.ToString() + "。对应数值" + (int)flags);
            Output.WriteLine("通过数字字符串转换");
            flags = (BxShopFlagsEnum)Enum.Parse(typeof(BxShopFlagsEnum), "5");
            Output.WriteLine(flags.ToString() + "。对应数值" + (int)flags);
            flags ^= BxShopFlagsEnum.非车商城h5;
            Output.WriteLine(flags.ToString() + "。对应数值" + (int)flags);

            //sql查看是否包含 SELECT * FROM `bx_agent` where (bxShop & 1) > 0
            Assert.True((flags & BxShopFlagsEnum.显示微信) > 0);
        }
    }
}
