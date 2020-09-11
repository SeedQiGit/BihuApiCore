using BihuApiCore.Model.Models;
using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IReflexService
    {
        Task<BaseResponse> XianZhongF(XianZhongF request);
        Task<BaseResponse> XianZhongG(GXianZhong request);
    }
}
