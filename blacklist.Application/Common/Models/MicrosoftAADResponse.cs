using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
	public class MicrosoftAADResponse
	{
		public string[] businessPhones { get; set; }
		public string displayName { get; set; }
		public string givenName { get; set; }
		public string jobTitle { get; set; }
		public string mail { get; set; }
		public string mobilePhone { get; set; }
		public string officeLocation { get; set; }
		public string preferredLanguage { get; set; }
		public string surname { get; set; }
		public string userPrincipalName { get; set; }
		public string Id { get; set; }
	}

	/*{
  "businessPhones": [
       "+1 425 555 0109"
   ],
   "displayName": "Adele Vance",
   "givenName": "Adele",
   "jobTitle": "Retail Manager",
   "mail": "AdeleV@contoso.onmicrosoft.com",
   "mobilePhone": "+1 425 555 0109",
   "officeLocation": "18/2111",
   "preferredLanguage": "en-US",
   "surname": "Vance",
   "userPrincipalName": "AdeleV@contoso.onmicrosoft.com",
   "id": "87d349ed-44d7-43e1-9a83-5f2406dee5bd"
}*/
}
