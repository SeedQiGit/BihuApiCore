using System;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BihuApiCore.Service.Implementations
{
    public class LoginService:ILoginService
    {
        private readonly ILoginInfoRepository _loginInfoRepository;
        private readonly IHttpContextAccessor _accessor;

        public LoginService(ILoginInfoRepository loginInfoRepository,IHttpContextAccessor accessor)
        {
            _loginInfoRepository = loginInfoRepository;
            _accessor = accessor;
        }

        public async Task Add()
        {						
            var browerAndOs = _accessor.HttpContext.GetBrowserAndOs();
            var ip = _accessor.HttpContext.GetClientIp();
            var record = new LoginInfo
            {
                Id=Guid.NewGuid().ToString(),
                OsName = browerAndOs.Os.Name,
                OsVerson = browerAndOs.Os.Version,
                BrowerMajor = browerAndOs.Browser.Major,
                BrowerName = browerAndOs.Browser.Name,
                BrowerVersion = browerAndOs.Browser.Version,
                Ip = ip,
                LoginTime = DateTime.Now,
                UserId =Guid.NewGuid().ToString("D"),
                UserName ="test"
            };

            await _loginInfoRepository.InsertAsync(record);
            await _loginInfoRepository.SaveChangesAsync();
			
        }


    }
}
