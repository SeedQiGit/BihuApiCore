using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IUserService
    {
        BaseResponse Test();
        Task<BaseResponse> TestAsy();
        Task<BaseResponse> MockAsy();
        Task<BaseResponse> AddUserByAccount(AddUserByAccountRequest request);
        Task<BaseResponse> TestEf();
        Task<BaseResponse> TestEf2();
    }
}
