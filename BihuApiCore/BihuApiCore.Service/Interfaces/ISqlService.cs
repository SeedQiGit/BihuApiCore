using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface ISqlService
    {
        Task<BaseResponse> TestTransaction();
        Task<BaseResponse<PageData<User>>> GetUserList(PageRequest request);
        Task<BaseResponse> TestSql();
        Task<BaseResponse> TestCompareValueAndassign();
    }
}
