using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public class AddApiVersioningHelper
    {
        public static void AddApiVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(p =>
            {

                p.AssumeDefaultVersionWhenUnspecified = true;
                p.DefaultApiVersion = new ApiVersion(1, 0);
                p.ReportApiVersions = true;


            }).AddVersionedApiExplorer(o => {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            
            });
        }
    }
}
