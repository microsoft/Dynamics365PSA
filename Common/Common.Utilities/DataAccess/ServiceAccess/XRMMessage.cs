using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Common.Utilities.DataAccess.ServiceAccess
{
    [DataContract]
    public class XRMMessage<TEntity>
    {
        [JsonProperty("d")]
        protected XRMEntitySet<TEntity> EntitySet { get; set; }

        public TEntity[] getResult()
        {
            return EntitySet.results;
        }
    }
}
