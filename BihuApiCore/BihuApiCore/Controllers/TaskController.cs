using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;

        }

        /// <summary>
        /// 测试get 同步  重试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Route("Test")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse>  Test()
        {
            for (int i = 0; i < 100; i++)
            {
                ModulusClass item = new ModulusClass();
                item.Key = i % 5;
                item.Value = i;
                Worker.WorkQueue.Enqueue(item);
            }
            LogHelper.Info(Worker.WorkQueue.Count.ToString());

            List<Task> taskList = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                Worker worker = new Worker();

                //如果在异步代码中直接赋值i，i都会是5。。。。。哈哈
                int a = i;
                //如果直接用worker.Run(); 不会新起现场，而是还是用主线程执行。
                //Task.Run(async () =>
                //{
                //    await worker.Run(a);
                //});


                //这样写会变成等待，也就是单线程
                //taskList.Add(worker.Run(a));
                //这样写会变成等待，也就是单线程
                //Task b=worker.Run(a);

                //这样使用w才能异步执行，并且把任务加到taskList，为啥正常的taskList.Add(worker.Run());就变成了同步代码呢？不懂
                var w = Task.Run(async () =>
                {
                    //这里循环后打印出来taskList.Count是0，说明for循环更快的完成了
                    //taskList.Add(worker.Run(a));
                    var b = worker.Run(a);
                });

                taskList.Add(w);

            }

            //这里打印出来的count 是0，说明for循环更快的完成了
            LogHelper.Info(taskList.Count.ToString());
            await Task.WhenAll(taskList);


            return BaseResponse.Ok();
        }


    }
    public class ModulusClass
    {
        public int Key { get; set; }

        public int Value { get; set; }
    }
    public class Worker
    {
        //定义队列
        public static ConcurrentQueue<ModulusClass> WorkQueue = new ConcurrentQueue<ModulusClass>();

        public async Task Run(int name)
        {
            SpinWait spin = new SpinWait();
            while (WorkQueue.Any())
            {
                ModulusClass item;

                while (!WorkQueue.TryDequeue(out item))
                {
                    spin.SpinOnce();
                }

                LogHelper.Info($"{item.Key}:{item.Value}");
                Thread.Sleep(500);
            }
             LogHelper.Info($"完成{name}");
        }
    }
}
