using BihuApiCore.Infrastructure.Helper.Http;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class BaiduController:ApiBaseController
    {

        private readonly DefaultClient _defaultClient;
       
        public BaiduController(DefaultClient defaultClient)
        {
            _defaultClient = defaultClient;          
        }

        /// <summary>
        /// 表单请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("BaiduOauth")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> BaiduOauth()
        {	
            #region 获取token

            //string token = (await _database.StringGetAsync(ConfigurationManager.GetValue("BaiduOauth"))).ToString();
            //if (string.IsNullOrWhiteSpace(token))
            //{
            //    var res=await BaiduOauth();
            //    if (res.Code!=1)
            //    {
            //        return res;
            //    }
            //    token=res.Data["access_token"].ToString();
            //    await _database.StringSetAsync(ConfigurationManager.GetValue("BaiduOauth"), token, TimeSpan.FromSeconds((int)res.Data["expires_in"]));
            //}

            #endregion


            String url = "https://aip.baidubce.com/oauth/2.0/token";
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", "dWEp78bfAvpGmcqKe7a5qY8y"));
            paraList.Add(new KeyValuePair<string, string>("client_secret", "XHz3WLzlbSInHfaLNUe4IGTgvFBvjopB"));

            string result =await _defaultClient.Client.HttpClientFromAsync(url,paraList);
            var jObject = JObject.Parse(result);

            if ( jObject["error"]!=null)
            {
                return BaseResponse.Failed(jObject["error"].ToString()+";"+jObject["error_description"].ToString());
            }; 

            return BaseResponse.Ok(result);
        }

        /// <summary>
        /// 表单请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("VehicleLicense")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> VehicleLicense( )
        {				
            String url = "https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_license?access_token=24.c469e03b92fd4fd5a8e61da8a3e2244c.2592000.1579330537.282335-17066373";
            
            //读图片转为Base64String
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(Path.Combine("D:\\", "1.png"));
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
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("image", userPhoto));
            paraList.Add(new KeyValuePair<string, string>("detect_direction", "false"));
            paraList.Add(new KeyValuePair<string, string>("accuracy", "normal"));
            paraList.Add(new KeyValuePair<string, string>("vehicle_license_side", "front"));

            string result =await _defaultClient.Client.HttpClientFromAsync(url,paraList);
            var jObject = JObject.Parse(result);

            if ( jObject["error_code"]!=null)
            {
                return BaseResponse.Failed(jObject["error_code"].ToString()+";"+jObject["error_msg"].ToString());
            }; 

            return BaseResponse.Ok(result);
        }

        /// <summary>
        /// 表单请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("VehicleLicenseObject")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> VehicleLicenseObject( )
        {				
            String url = "https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_license?access_token=24.c469e03b92fd4fd5a8e61da8a3e2244c.2592000.1579330537.282335-17066373";
            
            //读图片转为Base64String
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(Path.Combine("D:\\", "1.png"));
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

            //百度要求对base64进行编码
            var encodedPhoto=System.Web.HttpUtility.UrlEncode(userPhoto);
            object data = new { image = encodedPhoto ,detect_direction="false",accuracy="normal",vehicle_license_side="front"};

            string result =await _defaultClient.Client.FromCodedObjectAsync(url,data);
            var jObject = JObject.Parse(result);

            if ( jObject["error_code"]!=null)
            {
                return BaseResponse.Failed(jObject["error_code"].ToString()+";"+jObject["error_msg"].ToString());
            }; 

            return BaseResponse<object>.Ok(jObject);
        }

        /// <summary>
        /// 获取行驶本的Base64
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(BaseResponse), 1)]
        [HttpPost("Base64Photo")]
        public async Task<BaseResponse> Base64Photo()
        {
            //读图片转为Base64String
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(Path.Combine("D:\\", "1.png"));
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
