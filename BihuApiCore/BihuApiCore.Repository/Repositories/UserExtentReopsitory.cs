using System;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class UserExtentReopsitory:EfRepositoryBase<UserExtent>,IUserExtentReopsitory
    {
        public bihu_apicoreContext Context { get; set; }
        public UserExtentReopsitory(bihu_apicoreContext context) : base(context)
        {
            Context = context;
        }

        public void DelUserExtent(long userId)
        {
            var sql11 = $@"delete from user_extent where UserId = {userId} ";
            Context.Database.ExecuteScalar(sql11);
        }
        public void DelUserExtentContext(UserExtent user)
        {
            Context.UserExtent.Remove(user);
            Context.SaveChanges();
        }
    }
}
