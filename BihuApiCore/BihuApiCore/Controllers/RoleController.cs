﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    public class RoleController: BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService )
        {
            _roleService = roleService;
        }

        [HttpPost]
        public BaseResponse RoleModuleByRoleId([FromBody]RoleModuleByRoleIdRequest request)
        {
            var a =_roleService.RoleModuleByRoleId(request.RoleId,request.CompId);
            return BaseResponse<object>.GetBaseResponse(BusinessStatusType.OK,a);
        }

        [HttpPost]
        public async Task<BaseResponse> RoleModuleByRoleIdAsync([FromBody]RoleModuleByRoleIdRequest request)
        {
            return BaseResponse<List<ModuleTreeViewModel>>.GetBaseResponse(BusinessStatusType.OK,await _roleService.RoleModuleByRoleIdAsync(request.RoleId,request.CompId));
        }

        [HttpPost]
        public BaseResponse OldRoleModuleByRoleId([FromBody]RoleModuleByRoleIdRequest request)
        {
            var a =_roleService.OldRoleModuleByRoleId(request.RoleId,request.CompId);
            return BaseResponse<object>.GetBaseResponse(BusinessStatusType.OK,a);
        }
    }
}
