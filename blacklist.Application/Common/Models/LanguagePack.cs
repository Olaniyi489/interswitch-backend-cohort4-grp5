using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
    public class LanguagePack
    {
        public LanguagePack(string defaultMessage)
        {
            DefaultMessage = defaultMessage;
        }
        public string DefaultMessage { get; set; }
        public IDictionary<string, string> Mappings { get; set; }

    }
}
