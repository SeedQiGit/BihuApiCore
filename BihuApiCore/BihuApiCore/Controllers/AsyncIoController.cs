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

        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> Test( )
        {
            return await _asyncIoService.AsyncIoExcel().ConfigureAwait(false);
        }
    
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
