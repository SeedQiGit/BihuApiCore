using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BihuApiCore.Controllers
{
    /// <summary>
    /// 异步io控制器
    /// </summary>
    public class AsyncIoController: BaseController
    {
        private readonly IAsyncIoService _asyncIoService;
   
        public AsyncIoController(IAsyncIoService asyncIoService)
        {
            _asyncIoService = asyncIoService;
        }

        #region 小程序及二维码图片

        /// <summary>
        /// 传入连接字符串 获取对应的二维码图片Base64
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), 1)]
        [AllowAnonymous]
        [HttpGet("WeiChatGetQRCodeBase64")]
        public async Task<BaseResponse> WeiChatGetQRCodeBase64([FromQuery]string content)
        {
            //string content="ASDASDSA";
            QRCodeEncoder qrEncoder = new QRCodeEncoder();
            //二维码类型
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //二维码尺寸
            qrEncoder.QRCodeScale = 4;
            //二维码版本
            qrEncoder.QRCodeVersion = 7;
            //二维码容错程度
            qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //字体与背景颜色
            qrEncoder.QRCodeBackgroundColor = Color.White;
            qrEncoder.QRCodeForegroundColor = Color.Black;
            //UTF-8编码类型
            Bitmap qrcode = qrEncoder.Encode(content, Encoding.UTF8);

            //保存图片,指定保存 格式为Jpeg，占用空间会比较小
            //qrcode.Save("D:\\temp1.jpg",ImageFormat.Jpeg);
            //qrcode.Dispose();
            //保存图片数据    
            MemoryStream stream = new MemoryStream();
            qrcode.Save(stream, ImageFormat.Jpeg);
        
            return BaseResponse.Ok(Convert.ToBase64String(stream.ToArray()));
        }

        /// <summary>
        /// 小程序获取行驶本
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        [HttpPost("GetEtOcr")]
        public async Task<BaseResponse> GetEtOcr([FromBody]string data)
        {
            var url ="http://123.56.207.191/OcrWeb/EtOcr";

            //string dic;
            //var dic =  GetRequestPost();
           
            //读图片转为Base64String
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(Path.Combine("D:\\", "1.png"));
            string  userPhoto;
            using (MemoryStream ms1 = new MemoryStream())
            {
                bmp1.Save(ms1, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr1 = new byte[ms1.Length];
                ms1.Position = 0;
                ms1.Read(arr1, 0, (int)ms1.Length);
                ms1.Close();
                userPhoto = Convert.ToBase64String(arr1);
            }

            var dic=$"filedata={userPhoto}&pid=5";
            return BaseResponse.Ok(dic);
        }
        
        /// <summary>
        /// 获取post的参数组
        /// </summary>
        /// <returns></returns>
        private string GetRequestPost()
        {
            StringBuilder query = new StringBuilder("");
            var coll = Request.Form;
            foreach (var item in coll)
            {
                query.Append($@"{item.Key}={item.Value}&");
            }
            query = query.Remove(query.Length - 1, 1);
            return query.ToString();
        }

        #endregion
  
        #region Io方法

        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public  BaseResponse SyncIoExcel()
        {
            return  _asyncIoService.SyncIoExcel();
        }

        /// <summary>
        /// 异步方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> AsyncIoExcel()
        {
            return await _asyncIoService.AsyncIoExcel();
        }

        #endregion

        #region 多线程方法

        /// <summary>
        /// 监视多线程执行
        /// </summary>
        /// <param name="compId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> Monitor(long compId)
        {
            Task[] tasklist = new Task[10];
            Dictionary<object, object> dic = new Dictionary<object, object>();
            for (int i = 0; i < 10; i++)
            {
                Task task = new Task(a =>
                {
                    System.Threading.Thread.Sleep(1000);
                    dic.Add(a, a);
                }, i);
 
                tasklist[i] = task;
                task.Start();
            }
 
            //用户监控其他线程的状态
            Task monitorTask = new Task(() =>
            {
                //监控线程的个数
                int i = 0;
                while (true)
                {
                    i = 0;
                    foreach (Task item in tasklist)
                    {
                        if (item.Status == TaskStatus.RanToCompletion)
                        {
                            i++;
                        }
                    }
                    if (i == 10)
                    {
                        Console.WriteLine("所有线程执行完毕" + dic.Count);
                        break;
                    }
                }
            });
            monitorTask.Start();
            Console.ReadLine(); 
            return BaseResponse.Ok();
        }

        public async Task<BaseResponse> StartWork(long compId)
        {
            Task<BaseResponse>[] taskArray = new Task<BaseResponse>[2];
            // 创建task并启动
            taskArray[0]= Task.Factory.StartNew(()=> BaseResponse.Ok());
            taskArray[1]= Task.Factory.StartNew(()=> BaseResponse.Ok());
            //会阻塞主线程，一直等待开辟的线程，直到所有子线程都执行完后再执行线程
            Task.WaitAll(taskArray);
          
            if (taskArray[0].Result.Code==1&& taskArray[1].Result.Code==1)
            {
                return BaseResponse.Ok();
            }
            else
            {
                return BaseResponse.Failed();
            }
        }

        public async Task<BaseResponse> StartWorkBatch(string compIds)
        {
            var compStrArray=compIds.Split(',');
            List<Task<BaseResponse>> taskList=new List<Task<BaseResponse>>();
      
            foreach (var item in compStrArray)
            {
                long compId=Convert.ToInt64(item);
                // 创建task
                taskList.Add(Task.Factory.StartNew(()=> BaseResponse.Ok()));
                taskList.Add(Task.Factory.StartNew(()=> BaseResponse.Ok()));
            }
            var array =taskList.ToArray();
            Task.WaitAll(array);

            foreach (var item in array)
            {
                if (item.Result.Code!=1)
                {
                    return BaseResponse.Failed();
                }
            }
            return BaseResponse.Ok();
        }

        #endregion
    }
}
