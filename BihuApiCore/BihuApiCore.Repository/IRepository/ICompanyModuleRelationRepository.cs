using BihuApiCore.EntityFrameworkCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BihuApiCore.Repository.IRepository
{
    public interface ICompanyModuleRelationRepository: IRepositoryBase<CompanyModuleRelation>
    {
        List<Modules> CompanyModuleFullById(long compId);
        Task<List<Modules>> CompanyModuleFullByIdAsync(long compId);
    }
}
