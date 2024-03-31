using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses
{
    public interface IMessageProvider
    {
        string GetMessage(string code, string language);
        string GetMessage(string language);
        //string GetNotificationMessage(OtpPurpose messageId, string language);
    }
}
