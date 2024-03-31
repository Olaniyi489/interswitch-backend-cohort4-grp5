

using System.ComponentModel;

namespace blacklist.Domain.Entities
{
    public partial class ApplicationUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Occupation { get; set; }
        public DateTime DateCreated { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public bool IsBlacklisted { get; set; }
        public string Department {  get; set; }
        public int LoginCount { get; set; }
        public long? CountryId { get; set; }
        [DefaultValue(true)]
        public bool CanRestPassword { get; set; }

        //public ICollection<Role> Roles { get; set; }


    }
}
