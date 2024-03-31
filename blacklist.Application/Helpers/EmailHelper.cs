using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public class EmailHelper
    {

        private readonly EmailServiceBinding _emailServiceBinding;
        private readonly IAppDbContext _context;
        private readonly IDbContextTransaction _trans;
        public EmailHelper(EmailServiceBinding emailServiceBinding, IAppDbContext context)
        {
            _emailServiceBinding = emailServiceBinding;
            _context = context;
            _trans = _context.Begin();
        }

        public async Task<EmailServiceResponse> SendMail(EmailServiceModel request)
        {
            var result = new EmailServiceResponse();
            using (var httpClient = new HttpClient())
            {
                var url = _emailServiceBinding.BaseUrl;
                string path = _emailServiceBinding.PostMEssage;
                httpClient.BaseAddress = new Uri(url);
                request.scheduleDate = DateTime.Now;
                request.otherEmails = new List<OtherEmail>
                {
                    new OtherEmail
                    {
                        bbcRecieverEmail = "",
                        bbcRecieverName = "",
                        ccRecieverEmail = "",
                        ccRecieverName = ""
                    }
                                };
                var payLoad = JsonConvert.SerializeObject(request);

                StringContent content = new StringContent(payLoad, Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(path, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<EmailServiceResponse>(apiResponse);
                }
                bool status = false;
                if (result != null) { 
                    
                    status=  result.statusCode == "200" ? true : false; 
                
                }
                await _context.MessagingSystem.AddAsync(new MessagingSystem
                {
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                ,
                    IsActive = true,
                    Message = JsonConvert.SerializeObject(request),
                    MessageType = "",
                    Status = status
                });
                await _context.SaveChangesAsync();

                return result;
            }

        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

    }
}
