using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Utilities.DataAccess.ServiceAccess
{
    public class NetworkResponse
    {
        public HttpResponseMessage ResponseMessage { get; private set; }
        public bool IsSuccessStatusCode { get; private set; }

        public NetworkResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage != null)
            {
                this.ResponseMessage = responseMessage;
                this.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
            }
            else
            {
                this.IsSuccessStatusCode = false;
            }
        }

        /// <summary>
        /// Get the response based on given contract 
        /// </summary>
        /// <returns>If the status of the response is not correct, return the default value of the generic</returns>
        public async Task<TContract> GetResponse<TContract>()
        {
            TContract[] result = await this.GetResponseList<TContract>();
            if (result != null && result.Length > 0)
            {
                return result[0];
            }
            return default(TContract);
        }

        /// <summary>
        /// Get a list of objectes based on given contract 
        /// </summary>
        /// <returns>If the status of the response is not correct, return the default value of the generic</returns>
        public async Task<TContract[]> GetResponseList<TContract>()
        {
            if (this.IsSuccessStatusCode)
            {
                // DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<TContract>), new[]{typeof(TContract)});
                string content = await ResponseMessage.Content.ReadAsStringAsync();

                 // Deserialize response
                 XRMMessage<TContract> message = DataAccessUtil.DeserializeObject<XRMMessage<TContract>>(content);
                 return message.getResult();
            }
            return null;
        }
    }
}
