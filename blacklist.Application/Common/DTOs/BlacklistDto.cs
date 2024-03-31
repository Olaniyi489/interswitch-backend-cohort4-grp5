using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class BlacklistDto
    {
        public string? Category { get; set; }
        public string Reason { get; set; }
        public string? Email { get; set; }
        public DateTime BlacklistedAt { get; set; }
        public string? BlacklistedById { get; set; }
        public bool IsActive { get; set; }
    }
}
