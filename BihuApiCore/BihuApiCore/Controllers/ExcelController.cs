using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Threading.Tasks;
using System.Web;

namespace BihuApiCore.Controllers
{
    public class ExcelController:BaseController
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
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> ListToExcelFile()
        {						
            HttpContext.Response.Headers.Add("Site", "Simple-Talk");
            return await _excelService.ListToExcelFile();

        }

        /// <summary>
        /// 列表转换为本地文件，并返回绝对路径
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<IActionResult > ActionResultFile()
        {	
            var result=await _excelService.ListToExcelByte();
            //第一种:使用FileContentResult
            return File(result, "application/ms-excel", "fileContents.xls");
 
            //第二种:使用FileStreamResult
            //var fileStream = new MemoryStream(fileContents);
            //return File(fileStream, "application/ms-excel", "fileStream.xls");
 
            //第三种:使用FilePathResult
            //服务器上首先必须要有这个Excel文件,然会通过Server.MapPath获取路径返回.
            //var fileName = Server.MapPath("~/Files/fileName.xls");
            //return File(fileName, "application/ms-excel", "fileName.xls");
        }

        /// <summary>
        /// 列表转换为流，并通过流返回  使用这个方法要关闭日志中间件，不然会报错
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task ListToExcelByte()
        {
            var result=await _excelService.ListToExcelByte();
            Response.ContentType = "application/ms-excel;charset=UTF-8";
            //Response.ContentEncoding = Encoding.Default;
            if (Request.Headers["User-Agent"].IndexOf("Firefox") > -1)
            {
                Response.Headers.Add("Content-Disposition", string.Format(
                    "attachment;filename={0}", "导出通话记录-" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));
            }
            else
            {
                //这种直接加Headers在正常action里面没问题，但是我们这里就加不上，我也是醉了。
                //结果是因为Body.Write(result);要放在最后。。。。。
                Response.Headers.Add("Content-Disposition", string.Format(
                    "attachment;filename={0}", (HttpUtility.UrlEncode("导出通话记录-") + DateTime.Now.ToString("yyyyMMdd") + ".xls")));
            }
            HttpContext.Response.Headers.Add("Site", "Simple-Talk");
        
            ////直接一，简单粗暴，不要拼写错了就好~~
            //Response.Headers["Content-Disposition"] = "attachment; filename=fileContents.xls; filename*=UTF-8''fileContents.xls";
           
            //这个要放在最后，把上面设置的值都写进去
            Response.Body.Write(result);
            Response.Body.Flush();
            Response.Body.Close();
        }


    }
}



