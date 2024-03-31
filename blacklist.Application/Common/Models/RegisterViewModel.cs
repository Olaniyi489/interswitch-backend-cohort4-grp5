 


namespace blacklist.Application.Common.Models
{
   
    public class RegisterViewModel
    {
       
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public List<RegisterRole>  Role { get; set; }
      
      

     
        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, IHttpContextAccessor httpContext)
        {
            var lang = Convert.ToString(httpContext?.HttpContext?.Request?.Headers[ResponseCodes.LANGUAGE]);
            var response=new ValidationResponse();
            if (string.IsNullOrEmpty(Email))
            {
                var message = $"Email {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }
           
            if (string.IsNullOrEmpty(FirstName))
            {
                var message = $"Firtname {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }
            
            if (string.IsNullOrEmpty(LastName))
            {
                var message = $"Lastname {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }

            if (string.IsNullOrEmpty(Department))
            {
                var message = $"Department {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }
            if (Role == null || Role.Count <= 0)
            {
                var message = $"Role {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }

            source = response;
            return true;
        }

        public class RegisterRole
        {
            public string RoleName { get; set; }
        }
    }
}
