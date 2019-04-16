using BihuApiCore.Infrastructure.Helper;
using Xunit;

namespace BihuApiCore.Infrastructure.UnitTest.Helper
{
    public class LogHelperTest
    {
        [Fact]
        public void LogTest()
        {
            LogHelper.Info("12312 ");
        }
    }
}
