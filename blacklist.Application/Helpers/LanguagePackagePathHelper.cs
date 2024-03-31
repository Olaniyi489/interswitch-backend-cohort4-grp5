

namespace blacklist.Application.Helpers
{
    public class LanguagePackagePathHelper
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _config;

        public LanguagePackagePathHelper(IWebHostEnvironment hostEnvironment, IConfiguration config)
        {
            _hostEnvironment = hostEnvironment??throw new ArgumentNullException(nameof(hostEnvironment));
            _config = config??throw new ArgumentNullException(nameof(config));
        }

        public string GetLanguagePackagePath()
        {

            var path = $"{_hostEnvironment.ContentRootPath}{Path.DirectorySeparatorChar}{_config.GetValue<string>("SystemSettings:LanguagePackFolderName")}";
            return path ;
        }
    }
}

