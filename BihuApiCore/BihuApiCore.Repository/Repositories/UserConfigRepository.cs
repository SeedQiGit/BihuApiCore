﻿using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class UserConfigRepository : EfRepositoryBase<UserConfig>, IUserConfigRepository
    {
        public new bihu_apicoreContext Context { get; set; }

        public UserConfigRepository(bihu_apicoreContext context) : base(context)
        {
            Context = context;
        }

        public void DelUserConfig(long userId)
        {
            var sql = $@"delete from user_config where UserId = {userId} ";
            Context.Database.ExecuteSqlCommand(sql);
        }

        public void DelUserConfigContext(UserConfig user)
        {
            Context.UserConfig.Remove(user);
            Context.SaveChanges();
        }

    }
}
