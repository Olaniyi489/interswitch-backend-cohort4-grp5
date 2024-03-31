using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
    public class SystemSettings
    {
        public string ReturnFileUrl { get; set; }
        public string APIUrl { get; set; }
        public string CISWebURL { get; set; }
        public bool IsTest { get; set; }

        public string FileExtension { get; set; }
        public string FileLocalPath { get; set; }
        public string VideoLocalPath { get; set; }
    }
}
