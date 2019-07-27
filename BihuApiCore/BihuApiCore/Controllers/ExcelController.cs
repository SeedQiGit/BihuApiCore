using BihuApiCore.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    public class ExcelController:ApiBaseController
    {
        
        private readonly IExcelService _excelService;
       
        public ExcelController(IExcelService excelService)
        {
            _excelService = excelService;          
        }

        /// <summary>
        /// 列表转换为本地文件，并返回绝对路径
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListToExcelFile")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> ListToExcelFile()
        {						
            return await _excelService.ListToExcelFile();
        }

        ///// <summary>
        ///// 列表转换为流，并通过流返回
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("ListToExcelStream")]
        //[ProducesResponseType(typeof(BaseResponse), 1)]
        //public async Task<BaseResponse> ListToExcelStream()
        //{						
        //    return await _loginService.ListToExcelStream();
        //}

        
    }
}
