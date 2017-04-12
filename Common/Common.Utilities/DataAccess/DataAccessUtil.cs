using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.Utilities.DataAccess
{
    public static class DataAccessUtil
    {
        public static TContract DeserializeObject<TContract>(string data)
        {
            TContract result = JsonConvert.DeserializeObject<TContract>(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
            });

            return result;
        }

        public static string SerializeObject(object content)
        {
            string result = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
            });

            return result;
        }

        public static async Task<TResult> ReadFromStream<TResult>(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var content = await streamReader.ReadToEndAsync();

                try
                {
                    return DeserializeObject<TResult>(content);
                }
                catch (JsonReaderException ex)
                {
                    // If there is a deserialize error and the required content is actually a string, return the string as a whole
                    if (typeof(TResult) == typeof(string))
                    {
                        return (TResult)(Object)content;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        public static async Task<bool> WriteToStream(Stream stream, object content)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                var contentString = SerializeObject(content);
                await streamWriter.WriteAsync(contentString);
                return true;
            }
        }
    }
}
