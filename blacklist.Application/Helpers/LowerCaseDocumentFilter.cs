using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public class LowerCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var path=swaggerDoc.Paths.ToDictionary(entry=>string.Join('/',entry.Key.Split('/')
                .Select(x=>x.ToLower())),entry=>entry.Value);
            swaggerDoc.Paths = new OpenApiPaths();
            foreach((string key , OpenApiPathItem  value) in path)
            {
                foreach(OpenApiParameter parameter in value.Operations.SelectMany(o => o.Value.Parameters))
                {
                    parameter.Name=parameter.Name.ToLower();
                }
                swaggerDoc.Paths.Add(key, value);
            }
        }
    }
}
