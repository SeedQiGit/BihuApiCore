using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class AdoController:BaseController
    {
        private readonly IAdoService _adoService;
        

        public AdoController(IAdoService adoService)
        {
            _adoService = adoService;
        }

        [HttpGet]
        public async Task<BaseResponse> Command()
        {
            return await _adoService.SqlServerCommand();
        }

        [HttpGet]
        public async Task<BaseResponse> SqlServerDataReader()
        {
            return await _adoService.SqlServerDataReader();
        }

        [HttpGet]
        public async Task<BaseResponse> MysqlCommand()
        {
            return await _adoService.MysqlCommand();
        }

        [HttpGet]
        public async Task<BaseResponse> MysqlDataReader()
        {
            return await _adoService.MysqlDataReader();
        }

        [HttpGet]
        public async Task<BaseResponse> MysqlExecuteNonQuery()
        {
            return await _adoService.MysqlExecuteNonQuery();
        }
        [HttpGet]
        public async Task<BaseResponse> MysqlExecuteDataTable()
        {
            return await _adoService.MysqlExecuteDataTable();
        }
    }
}
