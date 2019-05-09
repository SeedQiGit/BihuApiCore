using BihuApiCore.EntityFrameworkCore.Models;

namespace BihuApiCore.Repository.IRepository
{
    public interface IUserRepository: IRepositoryBase<User>
    {
        void CommandTest();
    }
}
