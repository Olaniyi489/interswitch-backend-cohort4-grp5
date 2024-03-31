using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class DashBoardDTO
    {

        public int TotalProjects { get; set; }
        public int TotalDevelopers { get; set; }
        public int TotalAssignedProject { get; set; }
        public int TotalUnassignedProject { get; set; }
        public int TotalUnAssignedDeveloper { get; set; }       
        public int TotalAssignedDeveloper { get; set; }
        public List<CountLineOfBusiness> LineOfBusinessCount { get; set; }
       
        /*using System;


        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(Title))
            {
                string message = $"Title {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                string message = $"Description {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            if (string.IsNullOrWhiteSpace(DeveloperId))
            {
                string message = $"Developer {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            if (ProjectId <= 0)
            {
                string message = $"Project {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            source = response;
            return true;
        }

    }
*/
    }

    public class CountLineOfBusiness
    {
        public int Count { get; set; }
        public string Name { get; set; }
    }

   
}
