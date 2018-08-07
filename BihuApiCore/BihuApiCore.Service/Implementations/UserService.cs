using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public BaseResponse Test()
        {
            User userThis = _userRepository.FirstOrDefault(c=>c.Id==1);
            if (userThis == null)
            {
                return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "发起请求的用户不存在");
            }
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userThis);
        }
        public async Task<BaseResponse> TestAsy()
        {
            User userThis = await  _userRepository.FirstOrDefaultAsync(c => c.Id == 1);
            if (userThis == null)
            {
                return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "发起请求的用户不存在");
            }
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userThis);
        }
    }
}
