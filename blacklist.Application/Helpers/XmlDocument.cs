using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public class XmlDocument
    {
        public static void AddSwaggerXmlDocumentsHelper(SwaggerGenOptions options)
        {
            var xmlFileName = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "xml");
            options.IncludeXmlComments(xmlFileName, true);
        }
    }
}
