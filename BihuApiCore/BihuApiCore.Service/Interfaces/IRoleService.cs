using BihuApiCore.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IRoleService
    {
        List<ModuleTreeViewModel> RoleModuleByRoleId(long roleId, long compId);
        List<ManagerModuleViewModel> OldRoleModuleByRoleId(long roleId, long compId);
        Task<List<ModuleTreeViewModel>> RoleModuleByRoleIdAsync(long roleId, long compId);
    }
}
