using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Common.Utilities.Network
{
    public class Network : INetwork
    {
        public async Task<HttpResponseMessage> GetAsync(string requestUri,
                                                        List<HttpRequestContentType> contentTypes = null,
                                                        string bearerAuthentication = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    SetupHttpClient(httpClient, contentTypes, bearerAuthentication);
                    return await httpClient.GetAsync(requestUri);
                }
                // This exception occurs on Android when there is no network connection
                // Convert to 404 message to make it same behavior with Windows Phone
                catch (System.Net.WebException)
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        public async Task<HttpResponseMessage> GetAsyncAsJson(string requestUri, string bearerAuthentication = null)
        {
            return await GetAsync(requestUri, new List<HttpRequestContentType>() { HttpRequestContentType.Json }, bearerAuthentication);
        }

        private void SetupHttpClient(HttpClient httpClient, List<HttpRequestContentType> contentTypes = null, string bearerAuthentication = null)
        {
            if (contentTypes != null)
            {
                foreach (var contentType in contentTypes)
                {
                    switch (contentType)
                    {
                        case HttpRequestContentType.Json:
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            break;
                    }
                }
            }

            if (bearerAuthentication != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerAuthentication);
            }
        }
    }
}
