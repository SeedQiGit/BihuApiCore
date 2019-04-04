using BihuApiCore.EntityFrameworkCore.Models;

namespace BihuApiCore.Repository.IRepository
{
    public interface IUserExtentReopsitory: IRepositoryBase<UserExtent>
    {
        void DelUserExtent(long userId);
    }
}
