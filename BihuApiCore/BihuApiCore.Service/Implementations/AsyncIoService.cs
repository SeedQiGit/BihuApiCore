using System;
using System.IO;
using System.Net;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Helper;

namespace BihuApiCore.Service.Implementations
{
    public class AsyncIoService:IAsyncIoService
    {
        public BaseResponse SyncIoExcel()
        {
            string url = "http://image.91bihu.com//images/2019/3/12/user/de51f32e-60be-439f-9f17-a6e3302e1db7.xls";

            try
            {
                string storePath = "E:\\test.xlsx";
                string tempPath = Path.GetDirectoryName(storePath);
                Directory.CreateDirectory(tempPath);  //创建临时文件目录
                string tempFile = tempPath + @"\" + Path.GetFileName(storePath) + ".temp"; //临时文件
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);    //存在则删除
                }
                if (File.Exists(storePath))
                {
                    File.Delete(storePath);    //存在则删除
                }
                LogHelper.Info("创建临时文件：" + tempFile);
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                LogHelper.Info("移动临时文件到指定位置开始：" + storePath);
                File.Move(tempFile, storePath);
                LogHelper.Info("移动临时文件到指定位置完成：" + storePath);
            }
            catch (Exception ex)
            {
                LogHelper.Error("读取批量续保文件路径" + url + " 错误信息：" + ex.Message);
              
            }

            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);
        }


        public async Task<BaseResponse> AsyncIoExcel()
        {
           

            return BaseResponse.GetBaseResponse(BusinessStatusType.OK);

           
        }


    }
}
