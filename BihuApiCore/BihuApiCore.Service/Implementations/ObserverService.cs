using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace BihuApiCore.Service.Implementations
{
    public class ObserverService:IObserverService
    {
        private Action<long> _delAllSheetObserver;

        private readonly IUserRepository _userRepository;
        private readonly IUserExtentReopsitory _userExtentReopsitory;
        private readonly IUserConfigRepository _userConfigRepository;
        private readonly IMapper _mapper;

        public ObserverService(IUserRepository userRepository, IMapper mapper,IUserExtentReopsitory userExtentReopsitory, IUserConfigRepository userConfigRepository  )
        {
            _userExtentReopsitory = userExtentReopsitory;
            _userConfigRepository = userConfigRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _delAllSheetObserver += _userExtentReopsitory.DelUserExtent;
            _delAllSheetObserver += _userConfigRepository.DelUserConfig;
        }

        #region 新增用户及关联表

        public async Task<BaseResponse> AddUserAllSheet()
        {
            User user=new User
            {
                UserName="asd",
                UserPassWord="123123",
                CertificateNo="123131",
                UserAccount="123",
                Mobile=13313331333,
                IsVerify=1
            };
            _userRepository.Insert(user);
            //这里需要id，所以必须保存一下
            await _userRepository.SaveChangesAsync();
            UserExtent userExtent=new UserExtent
            {
                UserId=user.Id,
                UserHobby= "123123",
                UserOccupation= "123131",
            };
            _userExtentReopsitory.Insert(userExtent);

            UserConfig userConfig=new UserConfig
            {
                UserId=user.Id,
                UserLevel= 123,
                UserGrade= 123,
            };
            _userConfigRepository.Insert(userConfig);
            await _userConfigRepository.SaveChangesAsync();

            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
        }

        #endregion

        #region 观察者模式

        public async Task<BaseResponse> DelAllSheet(BaseRequest request)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                User user = _userRepository.FirstOrDefault(c => c.Id == request.UserId);
                if (user==null)
                {
                    return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
                }
                _userRepository.Delete(user);
                
                await _userRepository.SaveChangesAsync();
                _delAllSheetObserver(request.UserId);
                ts.Complete();
            }

            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
        }


        #endregion




       
    }
}
