using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class ZsPiccCallRepository:EfRepositoryBase<ZsPiccCall>,IZsPiccCallRepository
    {
        public ZsPiccCallRepository(DbContext context) : base(context)
        {
        }
    }
}
