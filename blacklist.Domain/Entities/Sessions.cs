﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Domain.Entities
{
    public class Sessions:BaseObject
    {
      
        public string Token { get; set; }         
        public string UserId { get; set; }
      
    }
}
