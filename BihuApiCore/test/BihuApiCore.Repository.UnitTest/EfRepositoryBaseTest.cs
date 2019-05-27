using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using BihuApiCore.EntityFrameworkCore;
using Xunit;

namespace BihuApiCore.Repository.UnitTest
{
    public class EfRepositoryBaseTest : IDisposable
    {
        private readonly DbContext _context;
        private readonly EfRepositoryBase<User> _repository;//用具体的泛型类型进行测试，这个不影响对EFRepository测试的效果

        public EfRepositoryBaseTest()
        {
            _context = new TestContext();
            _repository = new EfRepositoryBase<User>(_context);
        }

        [Fact]
        public void Test_insert_getbyid_table_tablenotracking_delete_success()
        {
            User user=new User
            {
                UserName="asd",
                UserAccount = "1233123213123",
                UserPassWord="123123",
                CertificateNo="123131",
                Mobile=13313331333,
                IsVerify=IsVerifyEnum.可用
            };
            _repository.Insert(user);
            _repository.SaveChanges();
            var newUserId = user.Id;
            Assert.True(newUserId > 0);

            //声明新的Context，不然查询直接由DbContext返回而不经过数据库
            using (var newContext = new TestContext())
            {
                var repository = new EfRepositoryBase<User>(newContext);
                var userInDb = repository.FirstOrDefault(c=>c.Id==newUserId);
                Assert.True(user.UserName==userInDb.UserName);
            }
            Assert.True( _context.Entry(user).State==EntityState.Unchanged);
            _repository.Delete(user);
            _repository.SaveChanges();
            using (var newContext = new TestContext())
            {
                var repository = new EfRepositoryBase<User>(newContext);
                var userInDb = repository.FirstOrDefault(c=>c.Id==newUserId);
                Assert.True( userInDb==null);
            }
        }

        [Fact]
        public void Test_insert_update_attach_success()
        {
            User user=new User
            {
                UserName="asd",
                UserAccount = "1233123213123",
                UserPassWord="123123",
                CertificateNo="123131",
                Mobile=13313331333,
                IsVerify=IsVerifyEnum.可用
            };
            _repository.Insert(user);
            _repository.SaveChanges();
            var newUserId = user.Id;
            Assert.True(newUserId > 0);

            //update
            using (var newContext = new TestContext())
            {
                var repository = new EfRepositoryBase<User>(newContext);
                var userInDb = repository.FirstOrDefault(c => c.Id == newUserId);
                userInDb.UserName = "lisi";
                repository.Update(userInDb);
                repository.SaveChanges();
            }

            //assert
            using (var newContext = new TestContext())
            {
                var repository = new EfRepositoryBase<User>(newContext);
                var userInDb = repository.FirstOrDefault(c => c.Id == newUserId);
                Assert.True(userInDb.UserName == "lisi");
            }

            _repository.Delete(user);
            _repository.SaveChanges();
        }

        public void Dispose()
        {
            var models = _repository.GetAllList();
            foreach (var item in models)
            {
                _repository.Delete(item);
            }
            _repository.SaveChanges();
            _context.Dispose();
        }
    }
}
