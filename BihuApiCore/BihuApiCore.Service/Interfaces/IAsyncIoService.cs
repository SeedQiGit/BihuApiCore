using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IAsyncIoService
    {
        BaseResponse SyncIoExcel();
        Task<BaseResponse> AsyncIoExcel();
        Task<BaseResponse> AsyncIoExcelWeb();
        Task<BaseResponse> AsyncIoExcelFile();
    }
}
