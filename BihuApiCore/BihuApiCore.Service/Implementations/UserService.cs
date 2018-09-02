using AutoMapper;
using BihuApiCore.EntityFrameworkCore;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Dto;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private UrlModel _urlModel;

        public UserService(IUserRepository userRepository, IMapper mapper, IProductRepository productRepository, ILogger<UserService> logger,IOptions<UrlModel> option)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _logger = logger;
            _urlModel = option.Value;
        }

        public BaseResponse Test()
        {

            User userThis = _userRepository.FirstOrDefault(w=>w.Id==1);
            UserDto userDto = _mapper.Map<UserDto>(userThis);
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userDto);
        }

        public async Task<BaseResponse> TestAsy()
        {
            //User userThis = await  _userRepository.FirstOrDefaultAsync(c => c.Id == 1);
            //if (userThis == null)
            //{
            //    return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "发起请求的用户不存在");
            //}
            //UserDto userDto = _mapper.Map<UserDto>(userThis);
            //return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userDto);

            string a  = "{ 'UserId':'10'}";
            string url = $"{_urlModel.BihuApi}/api/Message/MessageExistById";
            string result = await HttpWebAsk.HttpClientPostAsync(a, url);
           
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, result);
        }
    }
}
