using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;

namespace BihuApiCore.Controllers
{
    public class ImgController: BaseController
    {
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
            //QR图的大小(size)被定义为版本（Version)，版本号从1到40。版本1就是一个21*21的矩阵，每增加一个版本号，矩阵的大小就增 加4个模块(Module)，因此，版本40就是一个177*177的矩阵。（版本越高，意味着存储的内容越多，纠错能力也越强）。
            qrEncoder.QRCodeVersion = 10;
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

        /// <summary>
        /// 获取行驶本的Base64
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), 1)]
        [HttpPost("Base64Photo")]
        public async Task<BaseResponse> Base64Photo()
        {
            //读图片转为Base64String
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(Path.Combine("D:\\", "1.jpg"));
            string userPhoto;
            using (MemoryStream ms1 = new MemoryStream())
            {
                bmp1.Save(ms1, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr1 = new byte[ms1.Length];
                ms1.Position = 0;
                ms1.Read(arr1, 0, (int)ms1.Length);
                ms1.Close();
                userPhoto = Convert.ToBase64String(arr1);
            }
            return BaseResponse.Ok(userPhoto);
        }

    }
}
