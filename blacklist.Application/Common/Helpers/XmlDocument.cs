 

namespace blacklist.Application.Common.Helpers
{
    public class XmlDocument
    {
        public static void AddSwaggerXmlCommentsHelper(SwaggerGenOptions o)
        {
            //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlFilename = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "xml");
           // o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            o.IncludeXmlComments(xmlFilename);
           
        }
      
    }
}
