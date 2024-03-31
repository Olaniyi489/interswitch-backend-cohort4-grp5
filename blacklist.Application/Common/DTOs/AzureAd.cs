using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class AzureAd
    {
        public string Instance { get;set;}
        public string Domain { get;set;}
        public string ClientSecret { get;set;}
        public string TenantID { get;set;}
        public string ClientID { get;set;}
        public string CallbackUrl { get;set;}
        public string SignoutcallbackUrl { get;set;}
        public string ApplicationId { get;set;}
        public string ScopeName { get;set;}
        public DownstreamApi DownstreamApi { get; set;}
    }
   
    public class DownstreamApi
    {
        public string BaseUrl { get; set; }
        public string Scopes { get; set; }
    }
}
