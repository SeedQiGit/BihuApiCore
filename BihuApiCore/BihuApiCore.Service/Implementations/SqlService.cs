using System;
using System.Collections.Generic;
using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore;

namespace BihuApiCore.Service.Implementations
{
    public class SqlService:ISqlService
    {

        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;
     

        public SqlService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponse<PageData<User>>> GetUserList(PageRequest request)
        {
            var userThis = await _userRepository.FirstOrDefaultAsync(c => c.Id == request.UserId);
            if (userThis==null)
            {
                return BaseResponse<PageData<User>>.Failed("未找到对应公司",new PageData<User>());
            }
            return BaseResponse<PageData<User>>.Ok(await _userRepository.GetUserList(request,userThis.LevelCode));
        }

        public async Task<BaseResponse> TestTransaction()
        {
            using (var transaction =await _userRepository.GetDbContext().Database.BeginTransactionAsync())
            {
                await _userRepository.SaveChangesAsync();
                transaction.Commit();
            }
            return BaseResponse.Ok();
        }

        public async Task<BaseResponse> TestSql()
        {
            return BaseResponse<List<IsVerifyEnum>>.Ok( await _userRepository.TestSql());
        }

        public async Task<BaseResponse> TestCompareValueAndassign()
        {
            var userThis = await _userRepository.FirstOrDefaultAsync(c => c.Id == 6);
            UserCompareValueAndassign userCompareValueAndassign = new UserCompareValueAndassign
            {
                UserAccount = "top",
                UserName = "admin",
                IsVerify=IsVerifyEnum.删除,
                CreateTime = DateTime.Now
            };
            _userRepository.CompareValueAndassign(userThis,userCompareValueAndassign);
            await _userRepository.SaveChangesAsync();
            return BaseResponse.Ok();
        }


    }
}


