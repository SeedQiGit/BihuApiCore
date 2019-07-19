using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace BihuApiCore.Filters
{
    public class SwashbuckleOperationFilter: IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {      
            operation.Responses.Add("0", new Response { Description = "失败" });
            //operation.Responses.Add("1", new Response { Description = "成功" });
            var hasAuthorize = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>().Any() ||
                               context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });
                //这个控制是否对接口加锁，就是是附带token去请求
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { "oauth2", new [] { "business_stats" } }
                    }
                };
            }
            
        }
    }
}
