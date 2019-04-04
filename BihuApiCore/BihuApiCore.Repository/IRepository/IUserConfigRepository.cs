using System;
using System.Collections.Generic;
using System.Text;
using BihuApiCore.EntityFrameworkCore.Models;

namespace BihuApiCore.Repository.IRepository
{
    public interface IUserConfigRepository:IRepositoryBase<UserConfig>
    {
        void DelUserConfig(long userId);
    }
}
