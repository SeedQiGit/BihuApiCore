using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;

namespace BihuApiCore.Service.Interfaces
{
    public interface IObserverService
    {
        Task<BaseResponse> AddUserAllSheet();
        Task<BaseResponse> DelAllSheet(BaseRequest request);
    }
}
