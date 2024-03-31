using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Domain.Entities;

namespace blacklist.Domain.Entities
{
    public class Blacklist : BaseObject
    {
        public string? Category { get; set; }
        public string Reason { get; set; }
        public DateTime BlacklistedAt { get; set; }
        public string? Email { get; set; }
        public string? BlacklistedUserId { get; set; }
        [ForeignKey(nameof(BlacklistedUserId))]
        public virtual ApplicationUser? BlacklistedUserName { get; set; }
        public string? BlacklistedById { get; set; }
        [ForeignKey(nameof(BlacklistedById))]
        public virtual ApplicationUser? BlacklistedByName { get; set; }
    }
}
