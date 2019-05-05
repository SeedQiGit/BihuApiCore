using System;
using System.Collections.Generic;
using System.Text;
using BihuApiCore.EntityFrameworkCore.Models;

namespace BihuApiCore.Repository.IRepository
{
    public interface ICompanyModuleRelationRepository: IRepositoryBase<CompanyModuleRelation>
    {
        List<Modules> CompanyModuleFullById(long compId);
    }
}
