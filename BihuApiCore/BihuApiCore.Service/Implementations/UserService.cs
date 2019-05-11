using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Dto;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IDataExcelRepository _dataExcelRepository;
        private readonly IZsPiccCallRepository _zsPiccCallRepository;

        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private UrlModel _urlModel;

        private static ConcurrentDictionary<string, string> _strDic = new ConcurrentDictionary<string, string>();

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger, IOptions<UrlModel> option, IDataExcelRepository dataExcelRepository, IZsPiccCallRepository zsPiccCallRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;

            _logger = logger;
            _urlModel = option.Value;
            _dataExcelRepository = dataExcelRepository;
            _zsPiccCallRepository = zsPiccCallRepository;
        }

        #region 新增用户  层次码

        public async Task<BaseResponse> AddUser(BaseRequest request)
        {
            var userParent = await _userRepository.FirstOrDefaultAsync(c=>c.Id==request.UserId);
            if (userParent==null)
            {
                return BaseResponse.Failed("未找到对应父级用户");
            }
            User user = new User
            {
                UserName = "asd",
                UserAccount = "1233123213123",
                UserPassWord = "123123",
                CertificateNo = "123131",
                Mobile = 13313331333,
                IsVerify = 1,
                LevelNum = userParent.LevelNum+1,
                LevelCode = userParent.LevelCode,
                ParentId = userParent.Id
            };
            await _userRepository.InsertAsync(user);
            using (var transaction =await _userRepository.GetDbContext().Database.BeginTransactionAsync())
            {
                await _userRepository.SaveChangesAsync();
                user.LevelCode=$"{userParent.LevelCode}{user.Id},";//左右都加逗号是防止like到冗余数据
                await _userRepository.SaveChangesAsync();
                transaction.Commit();
            }

            return BaseResponse.Ok();
        }

        #endregion

        #region  修改用户  层次码

        public async Task<BaseResponse> UpdateUser(UpdateUserRequest request)
        {
            var user = await _userRepository.FirstOrDefaultAsync(c=>c.Id==request.UserId);
            if (user==null)
            {
                return BaseResponse.Failed("未找到对应用户");
            }
            
            //不重要的赋值就不模拟了

            if (request.ParentId!=user.ParentId)
            {
                //search parent data
                var userParent = await _userRepository.FirstOrDefaultAsync(c=>c.Id==request.ParentId);
                if (userParent == null) 
                {
                    return BaseResponse.Failed("未找到对应父级用户");
                }
                string newCode = $"{userParent.LevelCode}{user.Id},";
                string oldCode = user.LevelCode;
                int oldNum = user.LevelNum;
                int newNum = userParent.LevelNum + 1;
              
                int cNum = newNum - oldNum;
                user.LevelCode = newCode;
                user.LevelNum= newNum;
                user.ParentId = userParent.Id;
                //查找当前用户及其下级用户列表
                var relevantUsers = await _userRepository.GetAll().Where(x => x.LevelCode.StartsWith(oldCode)).ToListAsync();
                foreach (var item in relevantUsers)
                {
                    if (item.Id==user.Id)continue;
                   
                    item.LevelCode = item.LevelCode.Replace(oldCode, newCode);
                    item.LevelNum  = item.LevelNum + cNum;
                }
            }

            await _userRepository.SaveChangesAsync();
            return BaseResponse.Ok();
        }

        #endregion

        #region EF test

        public async Task<BaseResponse> TestEf()
        {
            //var a =await _userRepository.FirstOrDefaultAsync(c => c.Id == 6);
            //a.UserAccount = "1231";
            //_userRepository.SaveChanges();
            User a = new User();
            a.Id = 6;
            a.UserAccount = "121";
            //0.1添加到EF管理容器中，并获取 实体对象 的伪包装类对象
            EntityEntry<User> entry = _userRepository.GetDbContext().Entry<User>(a);
            ////如果使用 Entry 附加实体对象到数据容器中，则需要手动设置 实体包装类的对象的状态为 Unchanged
            ////**如果使用 Attach 就不需要这句
            //entry.State = EntityState.Unchanged;

            //标识实体对象某些属性已经被修改了
            entry.Property("UserAccount").IsModified = true;

            _userRepository.GetDbContext().Set<User>().Attach(a);
            //跟新到数据库
            _userRepository.SaveChanges();

            return BaseResponse<object>.Ok(a);
        }

        public async Task<BaseResponse> TestEf2()
        {
            User a = new User();
            a.Id = 6;
            a.UserAccount = "121";
            _userRepository.SetFieldValue(a,c=>c.UserAccount);
            _userRepository.AttachIfNot(a);
            //跟新到数据库
            _userRepository.SaveChanges();
            return BaseResponse<object>.Ok(a);
        }

        #endregion

        public int Add(int nb1, int nb2)
        {
            return nb1 + nb2;
        }

        public BaseResponse Test()
        {
            //User userThis;
            //using (bihu_apicoreContext ef = new bihu_apicoreContext())
            //{
            //    userThis = ef.User.FirstOrDefault();
            //}
            ////userThis = _userRepository.FirstOrDefault(w=>w.Id==1);
            //UserDto userDto = _mapper.Map<UserDto>(userThis);
            //return BaseResponse<UserDto>.GetBaseResponse(BusinessStatusType.OK, userDto);

            //_companyModuleRelationRepository.DeleteRelation(request.EditCompId);

            using (var transaction = _zsPiccCallRepository.GetDbContext().Database.BeginTransaction())
            {
                User user = new User
                {
                    UserName = "asd",
                    UserAccount = "1233123213123",
                    UserPassWord = "123123",
                    CertificateNo = "123131",
                    Mobile = 13313331333,
                    IsVerify = 1
                };
                _userRepository.Insert(user);
                ZsPiccCall picc = new ZsPiccCall();
                picc.UserName = "";
                picc.CallPassword ="1231231";
                picc.CallExtNumber = "12312321";
                picc.CallNumber = "12312";
                picc.CallId = 0;
                picc.UserAgentId = 0;
                picc.CallState = 1;
                picc.CreateTime = DateTime.Now;
                picc.UpdateTime = DateTime.Now;

                _userRepository.GetDbContext().Database.ExecuteSqlCommand(new RawSqlString(" delete from user where user.Id=7"));

                //_userRepository.CommandTest();
                _zsPiccCallRepository.Insert(picc);
                _userRepository.FirstOrDefault(w=>w.Id==1);
                _zsPiccCallRepository.SaveChanges();
                transaction.Commit();
            }
            return BaseResponse.Ok();
           

        }

        public async Task<BaseResponse> TestAsy()
        {
            using (var transaction =await _zsPiccCallRepository.GetDbContext().Database.BeginTransactionAsync())
            {
                User user = new User
                {
                    UserName = "asd",
                    UserAccount = "1233123213123",
                    UserPassWord = "123123",
                    CertificateNo = "123131",
                    Mobile = 13313331333,
                    IsVerify = 1
                };
                await _userRepository.InsertAsync(user);

                await _userRepository.GetDbContext().Database.ExecuteSqlCommandAsync(new RawSqlString(" delete from user where user.Id=8"));
                await _zsPiccCallRepository.SaveChangesAsync();
                transaction.Commit();
            }



            //var list = await _dataExcelRepository.GetAllListAsync();
            //if (userThis == null)
            //{
            //    return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "发起请求的用户不存在");
            //}
            //UserDto userDto = _mapper.Map<UserDto>(userThis);
            //List<ZsPiccCall> piccList=new List<ZsPiccCall>();
            //foreach (var item in list)
            //{
            //    ZsPiccCall picc = _mapper.Map<ZsPiccCall>(item);
            //    picc.CallPassword = picc.CallPassword.Substring(0, picc.CallPassword.Length - 2);
            //    picc.CallExtNumber = picc.CallExtNumber.Substring(0, picc.CallExtNumber.Length - 2);
            //    picc.CallNumber = picc.CallNumber.Substring(0, picc.CallNumber.Length - 2);
            //    picc.CallId = 0;
            //    picc.UserAgentId = 0;
            //    picc.CallState = 1;
            //    picc.CreateTime = DateTime.Now;
            //    picc.UpdateTime = DateTime.Now;
            //    _zsPiccCallRepository.Insert(picc);

            //}

            //_zsPiccCallRepository.SaveChanges();
            //return new BaseResponse(BusinessStatusType.OK);
            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);

            //string a  = "{ 'UserId':'10'}";
            //string url = $"{_urlModel.BihuApi}/api/Message/MessageExistById";
            //string result = await HttpWebAsk.HttpClientPostAsync(a, url);
        }

        public async Task<BaseResponse> MockAsy()
        {
            User user = new User
            {
                UserName = "asd",
                UserAccount = "1233123213123",
                UserPassWord = "123123",
                CertificateNo = "123131",
                Mobile = 13313331333,
                IsVerify = 1
            };
            _userRepository.Insert(user);
            var a = await _userRepository.SaveChangesAsync();
            //这里故意增加这个判断，为了测试mock
            if (a > 0)
            {
                return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
            }
            else
            {
                return BaseResponse.GetBaseResponse(BusinessStatusType.Failed);
            }
        }



        public async Task<BaseResponse> AddUserByAccount(AddUserByAccountRequest request)
        {
            if (_strDic.ContainsKey(request.Account))
            {
                return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "键值已存在");
            }
            //成功写入
            if (_strDic.TryAdd(request.Account, request.Account))
            {
                LogHelper.Info("成功写入");
                var userExist = await _userRepository.FirstOrDefaultAsync(c => c.UserAccount == request.Account);
                if (userExist == null)
                {
                    LogHelper.Info("新增账户：" + request.Account);
                    User user = new User
                    {
                        UserName = "asd",
                        UserPassWord = "123123",
                        CertificateNo = "123131",
                        Mobile = 13313331333,
                        IsVerify = 1
                    };

                    user.UserAccount = request.Account;

                    _userRepository.Insert(user);
                    _userRepository.SaveChanges();
                }
                string res;
                if (_strDic.TryRemove(request.Account, out res))
                {
                    LogHelper.Info("去除成功");
                }

                return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
            }
            //尝试竞争线程，写入失败
            return BaseResponse.GetBaseResponse(BusinessStatusType.Failed, "写入失败");
        }

    }
}
