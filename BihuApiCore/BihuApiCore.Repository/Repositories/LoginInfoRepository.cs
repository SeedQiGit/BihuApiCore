using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class LoginInfoRepository:EfRepositoryBase<LoginInfo>, ILoginInfoRepository
    {
        public LoginInfoRepository(DbContext context) : base(context)
        {
        }
    }
}
