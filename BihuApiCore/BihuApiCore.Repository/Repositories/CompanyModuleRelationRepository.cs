using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BihuApiCore.Repository.Repositories
{
    public class CompanyModuleRelationRepository: EfRepositoryBase<CompanyModuleRelation>, ICompanyModuleRelationRepository
    {
        public CompanyModuleRelationRepository(DbContext context) : base(context)
        {
        }

        #region 获取公司对应的权限模块

        public List<Modules> CompanyModuleFullById(long compId)
        {
            var sql = $@" SELECT modules.* FROM modules inner join company_module_relation as r on r.ModuleCode=modules.ModuleCode  where r.CompId={compId} and modules.IsUserd=1 ";
            return Context.SqlQuery<Modules>(sql);
        }

        public async Task<List<Modules>> CompanyModuleFullByIdAsync(long compId)
        {
            var sql = $@" SELECT modules.* FROM modules inner join company_module_relation as r on r.ModuleCode=modules.ModuleCode  where r.CompId={compId} and modules.IsUserd=1 ";
            return await Context.SqlQueryAsync<Modules>(sql);
        }

        #endregion
    }
}

