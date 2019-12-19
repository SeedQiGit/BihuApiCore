using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class LoggerController : BaseController
    {
        private readonly ILogger<LoggerController> _logger;
        private IHostingEnvironment _env;

        public LoggerController(ILogger<LoggerController> logger,IHostingEnvironment env)
        {
            _logger = logger;
           _env = env;
        }
        
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet("DownLoadFile")]
        public IActionResult DownLoadFile(string url)
        {
            try
            {
                var basepath = _env.ContentRootPath;
                string fullPath = Path.Combine(basepath, url);
                var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//推荐此方法

                string fileExt = Path.GetExtension(fullPath);
                //获取文件的ContentType
                var provider = new FileExtensionContentTypeProvider();
                var memi = provider.Mappings[fileExt];
                return File(stream, memi, Path.GetFileName(fullPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"导出文件失败：{url}");
                return NotFound();
            }
        }

        /// <summary>
        /// 查询文件
        /// </summary>
        [HttpGet("Get")]
        public async Task GetLogs()
        {
            var result = GetLogsMeth();
            var data = Encoding.UTF8.GetBytes(result);
            Response.ContentType = "text/html";
            await Response.Body.WriteAsync(data, 0, data.Length);
        }

        #region 日志下载

        private string logpath = "logs";
        private string GetLogsMeth()
        {
            var provider = _env.ContentRootFileProvider;
            var contents = provider.GetDirectoryContents(logpath);
            if (contents == null || !contents.Exists)
            {
                return "";
            }

            var result = new StringBuilder();
            string urlmap= "<a href=\"DownLoadFile?url={0}%2f{1}\" target=\"_blank\">{0}/{1}</a><br/>";
            foreach (var item in contents)
            {
                if (item.IsDirectory)
                {
                    result.AppendLine(item.Name);
                }
                else
                {
                    result.AppendLine(string.Format(urlmap, logpath, item.Name));
                }
            }
            return result.ToString();

        }
        #endregion
    }
}
