using BihuApiCore.Infrastructure.Bihu;
using Xunit;

namespace BihuApiCore.Infrastructure.UnitTest
{
    public class SecretCodeTest
    {
        [Fact(DisplayName = "加密码测试")]
        public void BaseTreeTest()
        {
            var strUrl = "EmployeeId=102&Md5=asdsadsadsada";

            SecretCodeTestClass testClass=new SecretCodeTestClass
            {
                EmployeeId=102,
                Md5="asdsadsadsada",
                SecretCode="asdasd"
            };
            Assert.True(strUrl.GetUrlMd5() == testClass.GetSecretCode());
        }
    }
    public class SecretCodeTestClass:ISecretCode
    {
        /// <summary>
        /// 员工id
        /// </summary>
        public long EmployeeId { get; set; }

        /// <summary>
        ///  md5标识
        /// </summary>
        public string Md5 { get; set; }

        /// <summary>
        ///  加密码
        /// </summary>
        public string SecretCode { get; set; }

    }
}
