using BihuApiCore.Model.Response;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Interfaces
{
    public interface IAdoService
    {
        Task<BaseResponse> SqlServerCommand();

        Task<BaseResponse> SqlServerDataReader();

        Task<BaseResponse> MysqlCommand();

        Task<BaseResponse> MysqlDataReader();

        Task<BaseResponse> MysqlExecuteNonQuery();

        Task<BaseResponse> MysqlExecuteDataTable();
    }
}
