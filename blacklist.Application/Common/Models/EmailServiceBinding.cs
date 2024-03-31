using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
    public class EmailServiceBinding
    {
        public string BaseUrl { get; set; }
        public string PostMEssage { get; set; }
        public string GetProjectCode { get; set; }
        public string GetProjectById { get; set; }
        public string AddUpdateProject { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public string ProjectCode { get; set; }
        public string SenderName { get; set; }
    }

}
