using BihuApiCore.Model.Models;
using System.Collections.Generic;

namespace BihuApiCore.Service.Interfaces
{
    public interface IRoleService
    {
        List<ModuleTreeViewModel> RoleModuleByRoleId(long roleId, long compId);
        List<ManagerModuleViewModel> OldRoleModuleByRoleId(long roleId, long compId);
    }
}
