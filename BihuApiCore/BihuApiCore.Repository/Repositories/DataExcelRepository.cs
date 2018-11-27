using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.Repository.Repositories
{
    public class DataExcelRepository:EfRepositoryBase<DataExcel>,IDataExcelRepository
    {
        public DataExcelRepository(DbContext context) : base(context)
        {
        }
    }
}
