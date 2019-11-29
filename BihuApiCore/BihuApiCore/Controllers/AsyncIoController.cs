using System;
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
    }
}
