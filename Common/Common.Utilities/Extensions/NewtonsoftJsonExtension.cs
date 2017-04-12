using Newtonsoft.Json.Linq;

namespace Common.Utilities.Extensions
{
    public static class NewtonsoftJsonExtension
    {
        public static bool HasAnyChildren(this JObject jObject, string key)
        {
            return jObject[key] != null && jObject[key].HasValues && jObject[key].First != null;
        }

        public static bool HasAnyChildren(this JToken jToken, string key)
        {
            return jToken[key] != null && jToken[key].HasValues && jToken[key].First != null;
        }
    }
}
