using BihuApiCore.EntityFrameworkCore.Models;
using System.Collections.Generic;

namespace BihuApiCore.Repository.IRepository
{
    public interface IRoleModuleRelationRepository: IRepositoryBase<RoleModuleRelation>
    {
        List<Modules> RoleModuleFullById(long roleId);
    }
}
