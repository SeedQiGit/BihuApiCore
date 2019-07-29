using System.IO;
using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IExcelService
    {
        Task<BaseResponse> ListToExcelFile();
        Task<MemoryStream> ListToExcelStream();
        Task<byte[]> ListToExcelByte();
    }
}

