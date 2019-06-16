using BihuApiCore.Infrastructure.Helper;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BihuApiCore.Infrastructure.UnitTest.Helper
{
    public class DistinctHelperTest
    {
        [Fact(DisplayName = "去重测试")]
        public void BaseTreeTest()
        {
            List<DistincTestCalss> list = new List<DistincTestCalss>();
            list.Add(new DistincTestCalss { Id = 0, UserAccount = "0010" });
            list.Add(new DistincTestCalss { Id = 1, UserAccount = "0010" });
            list.Add(new DistincTestCalss { Id = 2, UserAccount = "0010" });
            list.Add(new DistincTestCalss { Id = 3, UserAccount = "0010" });
            list.Add(new DistincTestCalss { Id = 4, UserAccount = "0010" });
            list.Add(new DistincTestCalss { Id = 0, UserAccount = "0010" });

            var distinctList = list.DistinctExtensions(x => x.Id).ToList();
            Assert.True(distinctList.Count() == 5);
            
            var distinctList2 = list.DistinctEx(x => x.UserAccount).ToList();
            Assert.True(distinctList2.Count() == 1);

            //测试多字段模式
            var distinctList3 = list.DistinctEx(x => x.Id,x => x.UserAccount).ToList();
            //Console.WriteLine(distinctList2.Count);
            Assert.True(distinctList3.Count() == 5);
        }
    }

}
