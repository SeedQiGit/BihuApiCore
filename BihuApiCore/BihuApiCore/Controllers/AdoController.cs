using System;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace BihuApiCore.Controllers
{
    public class AdoController:BaseController
    {
        private readonly IAdoService _adoService;
        private ILogger<AdoController> _logger;

        public AdoController(IAdoService adoService,ILogger<AdoController> logger)
        {
            _adoService = adoService;
            _logger=logger;
        }

        [HttpGet]
        public async Task<BaseResponse> Command()
        {
            try
            {
                var retryPolicy =
                    Policy
                        .Handle<Exception>()
                        .Retry(3, (ex, count) =>
                        {
                            _logger.LogError($"回访事件执行失败! 重试次数{count},处理异常：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
                        });
                await retryPolicy.Execute(async () =>
                {
                    await _adoService.SqlServerCommand();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"回访事件执行失败! 重试次数3次还是异常,处理异常：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
            }
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
