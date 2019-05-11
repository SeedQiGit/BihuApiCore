using BihuApiCore.Filters;
using BihuApiCore.Model.Dto;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IObserverService _observerService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger, IObserverService observerService)
        {
            _userService = userService;
            _logger = logger;
            _observerService = observerService;
        }

        #region levelCode相关测试接口
        
        #region 新增用户，非顶级

        [HttpPost]
        public async Task<BaseResponse> AddUser([FromBody]BaseRequest request)
        {
            return SetStatusCode(await _userService.AddUser(request));
        }

        #endregion

        #region 更新用户，非顶级

        [HttpPost]
        public async Task<BaseResponse> UpdateUser([FromBody]UpdateUserRequest request)
        {
            return SetStatusCode(await _userService.UpdateUser(request));
        }

        #endregion

        #endregion

        #region 测试接口

        /// <summary>
        /// 测试get 同步
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Route("Test")]
        [ProducesResponseType(typeof(UserDto), 1)]
        public BaseResponse Test()
        {
            return _userService.Test();
        }

        [HttpGet]
        public async Task<BaseResponse> TestEf()
        {
            return await _userService.TestEf();
        }
        
        [HttpGet]
        public async Task<BaseResponse> TestEf2()
        {
            return await _userService.TestEf2();
        }

        /// <summary>
        /// 测试get 异步
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Route("TestAsy")]
        public async Task<BaseResponse> TestAsy()
        {
            return await _userService.TestAsy();
            //return await Task.Run(()=> { return BaseResponse.GetBaseResponse(BusinessStatusType.OK); });
        }

        [HttpPost]
        [ModelVerifyFilter]
        public async Task<BaseResponse> TestPost([FromBody] BaseRequest request)
        {
            return await Task.Run(()=> { return BaseResponse.GetBaseResponse(BusinessStatusType.OK); });
        }

        [HttpPost]
        public async Task<BaseResponse> AddUserByAccount([FromBody] AddUserByAccountRequest request)
        {
            return await _userService.AddUserByAccount(request);
        }

        #endregion

        #region 观察者模式接口

        #region 创建账户 同时自动生成配置

        [HttpPost]
        [ModelVerifyFilter]
        public async Task<BaseResponse> AddUserAllSheet()
        {
            return await _observerService.AddUserAllSheet();
        }

        #endregion

        [HttpPost]
        [ModelVerifyFilter]
        public async Task<BaseResponse> DelAllSheet([FromBody]BaseRequest request)
        {
            return await _observerService.DelAllSheet(request);
        }

        #endregion

    }
}