using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class UserExtentReopsitory:EfRepositoryBase<UserExtent>,IUserExtentReopsitory
    {
        public UserExtentReopsitory(DbContext context) : base(context)
        {
        }
    }
}
