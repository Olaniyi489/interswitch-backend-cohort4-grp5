using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
   
        public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
        {
            private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

            public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider) =>
                _apiVersionDescriptionProvider = apiVersionDescriptionProvider;

            public void Configure(SwaggerGenOptions options)
            {
                foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateOpenApiInfo(description));
                }
            }

            private static OpenApiInfo CreateOpenApiInfo(ApiVersionDescription description)
            {

                var info = new OpenApiInfo()
                {
                    Title = SwaggerOptionsHelper.Title(),
                    Version = description.ApiVersion.ToString(),
                    Description = SwaggerOptionsHelper.Description(),
                    TermsOfService = string.IsNullOrWhiteSpace(SwaggerOptionsHelper.TermsUrl()) ? null : new Uri(SwaggerOptionsHelper.TermsUrl()),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact",
                        Url = string.IsNullOrWhiteSpace(SwaggerOptionsHelper.ContactUrl()) ? null : new Uri(SwaggerOptionsHelper.ContactUrl()),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = string.IsNullOrWhiteSpace(SwaggerOptionsHelper.LicenseUrl()) ? null : new Uri(SwaggerOptionsHelper.LicenseUrl()),
                    }
                };

                if (description.IsDeprecated)
                {
                    info.Description += " (deprecated)";
                }

                return info;
            }
        }
}
