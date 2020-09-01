using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                //=property.GetType();
                var valueSub = property.GetValue(request);
                var tpSub = valueSub.GetType();
                if (!ObjectExtession.IsValueType(tpSub) && valueSub != null)
                {
                    if (tpSub.IsSubclassOf(typeof(XianZhongBase)))
                    {
                        var propertiesSub = tpSub.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

                        //_logger.LogInformation(tpSub.Name);
                    }

                }
                

                //if (row.Table.Columns.Contains(property.Name))
                //{
                //    object objValue = row[property.Name];

                //    //忽略空值,忽略只读属性
                //    if (!ObjectExtession.IsNullOrDbNull(objValue) && property.CanWrite)
                //    {
                //        property.SetValue(tReturn, ObjectExtession.DbChangeType(objValue, property.PropertyType));
                //    }
                //}
            }





            return BaseResponse.Ok();
        }


    }
}
