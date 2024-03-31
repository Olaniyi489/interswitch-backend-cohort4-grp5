using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace blacklist.Application.Helpers
{
    public class SwaggerDefaultValue : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
           operation.Deprecated |= apiDescription.IsDeprecated();
            if(operation is null)
            {
                operation.Parameters=new List<OpenApiParameter>();
            }
            operation.Parameters.Add(new OpenApiParameter
            { 
                Name="language", In=ParameterLocation.Header,
                Required=true,
                Schema=new OpenApiSchema
                {
                    Type="string"
                }
            });
        }
    }
}
