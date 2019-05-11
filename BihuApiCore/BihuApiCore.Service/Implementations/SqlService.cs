using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    }
}


