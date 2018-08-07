using BihuApiCore.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IUserService
    {
        BaseResponse Test();
        Task<BaseResponse> TestAsy();
    }
}
