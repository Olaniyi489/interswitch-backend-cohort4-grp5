using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
    public class LanguageSettings
    {
        public string BaseLocation { get; set; }
        public BundleSettings[] Bundles { get; set; }
    }
}
