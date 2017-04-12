using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Xrm.Sdk.Samples;
using Microsoft.Xrm.Sdk.Query.Samples;
using System.Collections.Generic;
using System.Reflection;
using Common.Utilities.Extensions;

namespace Common.Utilities.DataAccess.ServiceAccess
{
    /// <summary>
    /// Singleton to provide OrganizationDataWebServiceProxy wrapper. Adding call to authentication and error handling feature.
    /// </summary>
    public class OrganizationServiceProxy : OrganizationDataWebServiceProxy
    {
        //redefine again webEndPoint and restEndPoint because in CRM SDK is private
        protected const string webEndpoint = "/XRMServices/2011/Organization.svc/web";
        protected const string restEndpoint = "/XRMServices/2011/OrganizationData.svc/";
        protected const string clientVersion = "SdkClientVersion=6.1.0.533";
        protected List<TypeInfo> typeList;        

        protected static OrganizationServiceProxy _current;
        public static OrganizationServiceProxy Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new OrganizationServiceProxy();
                }
                return _current;
            }
        }

        protected OrganizationServiceProxy():base()
        {
            typeList = new List<TypeInfo>();
        }

        public async Task<HttpResponseMessage> DiscoverAuthority(string serviceUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
                
                // need to specify soap endpoint with client version.
                HttpResponseMessage httpResponse = await httpClient.GetAsync(
                    String.Format("{0}{1}?{2}", serviceUrl, webEndpoint, clientVersion));

                return httpResponse;
            }
        }

        #region Rest calls
        /// <summary>
        /// Retrieve an array of entities with the given columns and filter
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="schemaName">Name of the entity to query</param>
        /// <param name="columnSet">set of columns to query, if null, then query all the columns</param>
        /// <param name="filter">filter condition, if null then query without conditions</param>
        /// <returns></returns>
        public async Task<TEntity[]> RestRetrieveCollection<TEntity>(string schemaName, ColumnSet columnSet, string filter=null)
            where TEntity : Entity
        {
            TEntity[] results = null;
            // Using a flag since I can't wait inside the exception
            
            try
            {
                // The URL for the OData organization web service.
                string ODataAction = String.Format("{0}Set", schemaName);
                if (columnSet != null)
                {
                    StringBuilder select = new StringBuilder();
                    foreach (string column in columnSet.Columns)
                    {
                        select.Append("," + column);
                    }
                    ODataAction = String.Format("{0}?$select={1}", ODataAction, select.ToString().Remove(0, 1) );
                }

                if (filter != null)
                {
                    ODataAction = String.Format("{0}&$filter={1}", ODataAction,filter );
                }
                    
                NetworkResponse response = await SendRequestAsync(ODataAction, HttpMethod.Get);
                results = await response.GetResponseList<TEntity>();
                
            }
            catch (Exception exception)
            {
                MessageCenter.ShowError(exception.Message).DoNotAwait();
            }

            return results;
        }

        #endregion

        /// <summary>
        /// Create a httpClient and sends the Get or Post request
        /// </summary>
        /// <param name="ODataAction"></param>
        /// <param name="requestMethod"></param>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        protected async Task<NetworkResponse> SendRequestAsync(string ODataAction, HttpMethod requestMethod, HttpContent requestContent = null)
        {
            HttpResponseMessage response = null;
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip }))
            {
                // Build and send the HTTP request.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    string uri = ServiceUrl + restEndpoint + ODataAction;
                    if (requestMethod == HttpMethod.Get)
                    {
                        response = await httpClient.GetAsync(uri);
                    }
                    else
                    {
                        response = await httpClient.PostAsync(uri, requestContent);
                    }
                }
                catch (Exception exception)
                {
                    MessageCenter.ShowError(exception.Message).DoNotAwait();
                }
            }
            return new NetworkResponse(response);
        }

        /// <summary>
        /// Retrieve a list of entities based on a given query using SOAP
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> RetrieveCollection<TEntity>(QueryBase query, IEnumerable<TypeInfo> relatedEntities = null)
            where TEntity : Entity
        {
            List<TEntity> response = null;
            
            // Add type info for early bound
            TypeInfo typeInfo = typeof(TEntity).GetTypeInfo();
            if (!typeList.Contains(typeInfo))
            {
                typeList.Add(typeInfo);
                if(relatedEntities != null)
                {
                    typeList.AddRange(relatedEntities);
                }
                OrganizationDataWebServiceProxy.types = typeList.ToArray();
            }
            EntityCollection entityCollection = await base.RetrieveMultiple(query);
            if (entityCollection != null && entityCollection.Entities != null)
            {
                response = new List<TEntity>();
                foreach (var entity in entityCollection.Entities)
                {
                    response.Add(entity as TEntity);
                }
            }
            return response;
        }
    }
}
