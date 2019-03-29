using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class UserConfigRepository : EfRepositoryBase<UserConfig>, IUserConfigRepository
    {
        public UserConfigRepository(DbContext context) : base(context)
        {
        }
    }
}
