using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Utilities.Office
{
    public class Office365
    {
        private static Office365 current;
        public static Office365 Current
        {
            get
            {
                if (current == null)
                {
                    current = new Office365();
                }
                return current;
            }
        }

        private Authentication.Authentication.AuthenticationResult discoveryAuthResult;
        private Dictionary<Office365Service, string> office365ServiceEndpoints;

        public string Office365DiscoveryServerUrl { get; private set; }

        public string DiscoveryQuery { get; private set; }

        private Office365()
        {
            Office365DiscoveryServerUrl = "https://api.office.com/discovery/";
            DiscoveryQuery = "https://api.office.com/discovery/me/services";
        }

        private async Task AuthenticateDiscoveryServiceAsync()
        {
            discoveryAuthResult = await Authentication.Authentication.Current.AcquireTokenByRefreshToken(Office365DiscoveryServerUrl);
        }

        /// <summary>
        /// Discovers Office 365 endpoints and add the endpoints to office365ServiceEndpoints dictionary
        /// </summary>
        /// <returns></returns>
        private async Task DiscoverOffice365EndpointsAsync()
        {
            if (discoveryAuthResult == null)
            {
                await AuthenticateDiscoveryServiceAsync();
            }

            office365ServiceEndpoints = new Dictionary<Office365Service, string>();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", discoveryAuthResult.AccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await httpClient.GetAsync(DiscoveryQuery);

            // If the request successes, extract the endpoints and add to the dictionary. Otherwise, ignore and do not add anything to the dictionary
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(jsonResult);

                Debug.WriteLine("Successfully authenticate with Delve");

                foreach (var item in result["value"])
                {
                    var serviceId = item["ServiceId"].ToString();
                    var serviceResourceId = item["ServiceResourceId"].ToString();

                    switch (serviceId)
                    {
                        case "O365_SHAREPOINT":
                            office365ServiceEndpoints[Office365Service.SharePoint] = serviceResourceId;
                            Debug.WriteLine("SharePoint URL: " + serviceResourceId);
                            break;
                    }
                }
            }
            else
            {
#if DEBUG
                Debug.WriteLine("Cannot authenticate with Delve. Error: " + response.ToString());
                if (Debugger.IsAttached) Debugger.Break();
#endif
            }
        }

        public async Task<string> GetServiceEndpointAsync(Office365Service service)
        {
            if (office365ServiceEndpoints == null)
            {
                await DiscoverOffice365EndpointsAsync();
            }

            if (office365ServiceEndpoints.ContainsKey(service))
            {
                return office365ServiceEndpoints[service];
            }
            else
            {
                return null;
            }
        }

        public bool LogOut()
        {
            discoveryAuthResult = null;
            office365ServiceEndpoints = null;
            return true;
        }
    }

    public enum Office365Service
    {
        SharePoint
    }
}
