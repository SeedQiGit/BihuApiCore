using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BihuApiCore.Repository.Repositories
{
    public class RoleModuleRelationRepository: EfRepositoryBase<RoleModuleRelation>,IRoleModuleRelationRepository
    {
        public RoleModuleRelationRepository(DbContext context) : base(context)
        {
        }

        #region 获取角色对应的权限模块

        public List<Modules> RoleModuleFullById(long roleId)
        {
            var sql = $@" SELECT modules.* FROM modules inner join role_module_relation as r on r.ModuleCode=modules.ModuleCode  where r.RoleId={roleId} ";
            return Context.SqlQuery<Modules>(sql);
        }

        #endregion

    }
}
