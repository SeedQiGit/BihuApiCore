using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.Repositories;
using System;
using System.Linq;
using Xunit;

namespace BihuApiCore.Repository.UnitTest.UserRepositoryTest
{
    //public class UserRepositoryTest: IDisposable
    //{
    //    private readonly UserRepository _userRepository;

    //    public UserRepositoryTest()
    //    {
    //        _userRepository=new UserRepository(new TestContext());
    //    }

    //    [Fact]
    //    public void Add_Ok()
    //    {
    //        User user=new User
    //        {
    //            UserName="asd",
    //            UserAccount = "aaaaa",
    //            UserPassWord="123123",
    //            CertificateNo="123131",
    //            Mobile=13313331333,
    //            IsVerify=1
    //        };

    //        _userRepository.Insert(user);
    //        _userRepository.SaveChanges();
    //        User model = _userRepository.GetAll().FirstOrDefault(t => t.UserAccount == "aaaaa");
    //        Assert.True(model != null);
    //    }

    //    public void Dispose()
    //    {
           
    //        var models = _userRepository.GetAllList();
    //        foreach (var item in models)
    //        {
    //            _userRepository.Delete(item);
    //        }
    //        _userRepository.SaveChanges();
    //    }
    //}
}
