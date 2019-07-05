using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BihuApiCore.Service.UnitTest
{
    public class UserServiceTest
    {
        private readonly UserService _service;
        public UserServiceTest()
        {
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(f => f.Insert(It.IsAny<User>()));
            userRepository.Setup(f => f.SaveChangesAsync()).Returns(Task.FromResult(1));
            var mapper = new Mock<IMapper>();
            var iLogger = new Mock<ILogger<UserService> >();
            var option = new Mock< IOptions<UrlModel>>();
            var dataExcelRepository = new Mock<IDataExcelRepository>();
            var zsPiccCallRepository = new Mock<IZsPiccCallRepository>();
       
            _service = new UserService(userRepository.Object,mapper.Object,iLogger.Object,option.Object,dataExcelRepository.Object,zsPiccCallRepository.Object);
        }

        [Theory]
        [InlineData(2, 3, 5)]
        [InlineData(2, 4, 6)]
        [InlineData(2, 1, 3)]
        public void Add_Mock_Ok(int nb1, int nb2, int result)
        {
            var isCreateOk = _service.Add(nb1,nb2);
            Assert.True(result==isCreateOk);
        }

        [Fact]
        public async Task MockAsy_Test()
        {
            var response =await _service.MockAsy();
            Assert.True(response.Code==1);
        }

        //[Fact]
        //public void Add_Mock_Ok(int nb1, int nb2, int result)
        //{
        //    var isCreateOk = _service.Add(nb1,nb2);
        //    Assert.True(result==isCreateOk);
        //}

    }
    
}
