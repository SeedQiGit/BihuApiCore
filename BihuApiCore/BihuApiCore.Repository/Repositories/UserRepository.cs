using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class UserRepository: EfRepositoryBase<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public void CommandTest()
        {
            var sql = $@" delete from user where user.Id=100 ";
            Context.Database.ExecuteScalar(sql);
        }


    }
}
