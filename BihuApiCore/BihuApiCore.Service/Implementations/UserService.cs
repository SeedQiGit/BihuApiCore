using System;
using System.Collections.Generic;
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
using System.Linq;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDataExcelRepository _dataExcelRepository;
        private readonly IZsPiccCallRepository _zsPiccCallRepository;


        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private UrlModel _urlModel;

        public UserService(IUserRepository userRepository, IMapper mapper, IProductRepository productRepository, ILogger<UserService> logger,IOptions<UrlModel> option,IDataExcelRepository dataExcelRepository, IZsPiccCallRepository zsPiccCallRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _logger = logger;
            _urlModel = option.Value;
            _dataExcelRepository = dataExcelRepository;
            _zsPiccCallRepository = zsPiccCallRepository;
        }

        public BaseResponse Test()
        {
            User userThis;
            using (EntityContext ef = new EntityContext())
            {
                userThis = ef.User.FirstOrDefault(w => w.Id == 1);
            }

            //userThis = _userRepository.FirstOrDefault(w=>w.Id==1);
            UserDto userDto = _mapper.Map<UserDto>(userThis);
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK, userDto);
        }

        public async Task<BaseResponse> TestAsy()
        {
            var list = await _dataExcelRepository.GetAllListAsync();
            //if (userThis == null)
            //{
            //    return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "发起请求的用户不存在");
            //}
            //UserDto userDto = _mapper.Map<UserDto>(userThis);
            //List<ZsPiccCall> piccList=new List<ZsPiccCall>();
            foreach (var item in list)
            {
                ZsPiccCall picc = _mapper.Map<ZsPiccCall>(item);
                picc.CallPassword = picc.CallPassword.Substring(0, picc.CallPassword.Length - 2);
                picc.CallExtNumber= picc.CallExtNumber.Substring(0, picc.CallExtNumber.Length - 2);
                picc.CallNumber= picc.CallNumber.Substring(0, picc.CallNumber.Length - 2);
                picc.CallId = 0;
                picc.UserAgentId = 0;
                picc.CallState = 1;
                picc.CreateTime =DateTime.Now;
                picc.UpdateTime =DateTime.Now;
                _zsPiccCallRepository.Insert(picc);

            }

            _zsPiccCallRepository.SaveChanges();






            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);






            //string a  = "{ 'UserId':'10'}";
            //string url = $"{_urlModel.BihuApi}/api/Message/MessageExistById";
            //string result = await HttpWebAsk.HttpClientPostAsync(a, url);
        }
    }
}
