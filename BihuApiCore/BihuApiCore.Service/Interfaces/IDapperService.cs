using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IDapperService
    {
        Task<BaseResponse> DapperGetList();
        Task<BaseResponse> DapperBulkInsert();
    }
}
