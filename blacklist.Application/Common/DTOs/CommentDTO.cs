using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class CommentDTO
    {
        public long? ProjectId { get; set; }
        public string Content { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();

            if (ProjectId <= 0)
            {
                string message = $"ProjectId {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            if (string.IsNullOrWhiteSpace(Content))
            {
                string message = $"Content {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            source = response;
            return true;
        }
        public class CommentsViewModel
        {
            public string Content { get; set; }
            public long CommentId { get; set; }
            public DateTime DateCreated { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}
