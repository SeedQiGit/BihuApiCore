using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Implementations;
using Moq;
using Xunit;

namespace BihuApiCore.Service.UnitTest
{
    public class SqlServiceTest
    {
        //具体测试某个方法时候，在调用构造函数，而且指定对应模拟方法的返回值
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public SqlServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock=new Mock<IMapper>();
        }

        [Fact]
        public async Task GetUserList_Test()
        {
            _userRepositoryMock.Setup(x => x.GetUserList(It.IsAny<PageRequest>(),It.IsAny<string>())).Returns(Task.FromResult<PageData<User>>(new PageData<User>())) ;
            _userRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(new User())) ;

            var sqlService = new SqlService(
                _userRepositoryMock.Object, _mapperMock.Object);

            var result=await sqlService.GetUserList(new PageRequest());
            Assert.True(result.Code==1);
        }

    }
}
