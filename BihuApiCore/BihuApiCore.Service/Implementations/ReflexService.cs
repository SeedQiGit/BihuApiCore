using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class ReflexService: IReflexService
    {
        private readonly ILogger<ReflexService> _logger;

        public ReflexService(ILogger<ReflexService> logger)
        {
            _logger = logger;
        }

        public async Task<BaseResponse> XianZhongF(XianZhongF request)
        {
            //_logger.LogInformation(JsonConvert.SerializeObject(request));
            //根据反射计算总金额
            double total = 0;
            Type tp = typeof(XianZhongF);
            //属性列表
            var properties = tp.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                var valueSub = property.GetValue(request);
              
                if (valueSub != null)
                {
                    var tpSub = valueSub.GetType();
                    if (tpSub.IsSubclassOf(typeof(XianZhongBase)))
                    {
                  
                        var propertiesSub = tpSub.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

                        var baoFeiProp = propertiesSub.Where(c => c.Name == "baoFei").FirstOrDefault();
                        var buJiMianBaoFeiProp = propertiesSub.Where(c => c.Name == "buJiMianBaoFei").FirstOrDefault();
                        var baoFei =  Convert.ToDouble(baoFeiProp.GetValue(valueSub));
                        var buJiMianBaoFei = Convert.ToDouble(buJiMianBaoFeiProp.GetValue(valueSub));
                        total += baoFei;
                        total += buJiMianBaoFei;
                    }

                }             
            }

            return BaseResponse.Ok(total.ToString());
        }


    }
}
