using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public static class SwaggerOptionsHelper
    {
        public static IConfiguration _config;
        public static void SwaggerOptionsHelperConfifure(IConfiguration configuration) => _config = configuration;

        #region Public Methods

        public static string TermsUrl() => _config["SwaggerConfig:TermsUrl"];
        public static string PolicyUrl() => _config["SwaggerConfig:PolicyUrl"];
        public static string LicenseUrl() => _config["SwaggerConfig:LicenseUrl"];
        public static string ContactUrl() => _config["SwaggerConfig:ContactUrl"];
        public static string Description() => _config["SwaggerConfig:Description"];
        public static string Title() => _config["SwaggerConfig:Title"];



        #endregion
    }
}
