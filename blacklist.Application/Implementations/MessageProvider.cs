using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Implementations
{
    public class MessageProvider : IMessageProvider
    {


        readonly ILanguageConfigurationProvider _provider;

        public MessageProvider(ILanguageConfigurationProvider provider)
        {
            _provider = provider;
        }

        public string GetMessage(string code, string language)
        {
            var bundle = _provider.GetPack(language);
            if (bundle == null)
            {
                return "Invalid language configuration";
            }
            if (bundle.Mappings.TryGetValue(code, out var message))
            {
                return message;
            }
            return bundle.DefaultMessage;

        }


        public string GetMessage(string language)
        {
            var bundle = _provider.GetPack(language);
            if (bundle == null)
            {
                return "Invalid language configuration";
            }
            return bundle.DefaultMessage;

        }
    }
}
