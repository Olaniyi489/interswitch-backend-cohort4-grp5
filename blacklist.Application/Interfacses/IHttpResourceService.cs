using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses
{
    public interface IHttpResourceService
    {
        Task<T> Get<T>(string endpoint);

        Task<T> Get<T>(string endpoint, object id);

        Task<T> Post<T>(string endpoint, object model);

        Task<T> Put<T>(string endpoint, object model);

        Task<T> Delete<T>(string endpoint, object id);

        Task<HttpResponseMessage> SendRequest(string endpoint, HttpMethod method, object body = null, bool isJson = true);
    }
}
