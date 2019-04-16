using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.Repositories;
using Xunit;

namespace BihuApiCore.Repository.UnitTest.UserRepositoryTest
{
    public class UserRepositoryTest: IDisposable
    {
        private readonly UserRepository _userRepository;

        public UserRepositoryTest()
        {
            _userRepository=new UserRepository(new bihu_apicoreContext ());
        }

        [Fact]
        public void Add_Ok()
        {
            User user=new User
            {
                UserName="asd",
                UserAccount = "aaaaa",
                UserPassWord="123123",
                CertificateNo="123131",
                Mobile=13313331333,
                IsVerify=1
            };

            _userRepository.Insert(user);
            _userRepository.SaveChanges();
            User model = _userRepository.GetAll().FirstOrDefault(t => t.UserAccount == "123");
            Assert.True(model != null);
        }

        public void Dispose()
        {
           
        }
    }
}
