using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Domain.Entities
{
    public class UserActivities:BaseObject
    {
         
        public string UserId { get; set; }       
        public bool IsActivityResult { get; set; }
        public string Activity { get; set; }
        public long? CountryId { get; set; }
        public string Action { get; set; }
    
    }
  
}
