using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Dto;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, IProductRepository productRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _logger = logger;
        }

        public BaseResponse Test()
        {
            _logger.LogInformation("开始执行");
            User userThis = _userRepository.FirstOrDefault(c=>c.Id==1);
            UserDto userDto = _mapper.Map<UserDto>(userThis);
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userDto);
        }
        public async Task<BaseResponse> TestAsy()
        {
            _logger.LogInformation("开始执行");
            User userThis = await  _userRepository.FirstOrDefaultAsync(c => c.Id == 1);
            if (userThis == null)
            {
                return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "发起请求的用户不存在");
            }
            UserDto userDto = _mapper.Map<UserDto>(userThis);
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userDto);
        }
    }
}
