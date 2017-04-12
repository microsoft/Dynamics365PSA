using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Common.Utilities.DataAccess.ServiceAccess
{
    [DataContract]
    public class XRMEntitySet<TEntity>
    {
        public TEntity[] results;

        [JsonProperty("results")]
        public TEntity[] Result
        {
            get
            {
                return results;
            }
            set
            {
                results = value;
            }
        }
    }
}
