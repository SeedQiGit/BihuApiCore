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
            }
            
        }
    }
}
