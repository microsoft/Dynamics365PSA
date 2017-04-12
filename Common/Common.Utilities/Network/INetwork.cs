using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Utilities.Network
{
    public interface INetwork
    {
        Task<HttpResponseMessage> GetAsync(string requestUri,
                                           List<HttpRequestContentType> contentTypes = null,
                                           string bearerAuthentication = null);
        Task<HttpResponseMessage> GetAsyncAsJson(string requestUri,
                                                 string bearerAuthentication = null);
    }
}
