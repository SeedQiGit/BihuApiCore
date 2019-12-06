using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IHttpService
    {
       Task<BaseResponse> FormRequest();
       Task<BaseResponse> PressureTest();
    }
}
