using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore;

namespace BihuApiCore.Repository.IRepository
{
    public interface IUserRepository: IRepositoryBase<User>
    {
        void CommandTest();
        Task<PageData<User>> GetUserList(PageRequest request, string levelCode);
        Task<List<IsVerifyEnum>> TestSql();
    }
}
