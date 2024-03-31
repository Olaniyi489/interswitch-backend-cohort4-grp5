using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
	public class SystemSettingsHelper
	{
		private   readonly IConfiguration _config;
		public SystemSettingsHelper(IConfiguration config)
		{
			_config = config??throw new ArgumentNullException(nameof(config));
		}
		public   string WebUrl()
		{
			string baseurl = _config.GetValue<string>("SystemSettings:WebBaseUrl");
			return baseurl;

		}
	}
}
