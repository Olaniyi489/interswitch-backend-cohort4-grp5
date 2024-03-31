using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
    public class EmailServiceModel
    {
        public string senderName { get; set; }
        public string recieverName { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string projectCode { get; set; }
        public string messageBody { get; set; }
        public List<OtherEmail> otherEmails { get; set; }
        public bool sentNow { get; set; }
        public DateTime? scheduleDate { get; set; }
    }
    public class OtherEmail
    {
        public string bbcRecieverEmail { get; set; }
        public string bbcRecieverName { get; set; }
        public string ccRecieverEmail { get; set; }
        public string ccRecieverName { get; set; }
    }
    public class EmailServiceResponse
    {
        public bool hasError { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
        public string token { get; set; }
        public Result result { get; set; }
    }
    public class Result
    {
        public string message { get; set; }
        public bool isSuccessful { get; set; }
        public int retId { get; set; }
        public string errors { get; set; }
    }

}
