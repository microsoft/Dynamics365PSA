using Common.Model.Map;
using Common.Utilities.Extensions;
using Common.Utilities.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Utilities.Map
{
    public class BingMapsUtil : IMapUtil
    {
        // The Bing Maps API used is documented here
        // Main Documentation: https://msdn.microsoft.com/en-us/library/ff701711.aspx
        // Status Codes and Error Handling: https://msdn.microsoft.com/en-us/library/ff701703.aspx
        private const string BingMapsGeocodeUrl = "https://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}";

        // The Bing Maps Key can be obtained from http://www.microsoft.com/maps/create-a-bing-maps-key.aspx
        private string bingMapsKey;

        private INetwork network;

        public BingMapsUtil(string bingMapsKey, INetwork network)
        {
            this.bingMapsKey = bingMapsKey;
            this.network = network;
        }

        public async Task<Coordinate> GetCoordinateFromAddressAsync(string address)
        {
            Coordinate coordinate = null;
            if (address != null)
            {
                try
                {
                    var bingGeocodeQuery = GetBingGeocodeQueryUri(address);
                    coordinate = await GetCoordinate(bingGeocodeQuery);
                }
                catch
                {
                    BreakDebuggerOnError();
                }
            }

            LogReturnedCoordinateForDebug(address, coordinate);
            return coordinate;
        }

        private string GetBingGeocodeQueryUri(string address)
        {
            var bingGeocodeQuery = string.Format(BingMapsGeocodeUrl, Uri.EscapeDataString(address), bingMapsKey);
            LogGeocodeQueryForDebug(address, bingGeocodeQuery);
            return bingGeocodeQuery;
        }

        private async Task<Coordinate> GetCoordinate(string bingGeocodeQuery)
        {
            var queryResponse = await network.GetAsyncAsJson(bingGeocodeQuery);
            if (queryResponse.IsSuccessStatusCode)
            {
                var resultJson = await queryResponse.Content.ReadAsStringAsync();
                return GetCoordinateFromBingQueryResult(resultJson);
            }

            return null;
        }

        private Coordinate GetCoordinateFromBingQueryResult(string bingQueryResultJson)
        {
            var bingQueryResult = JsonConvert.DeserializeObject(bingQueryResultJson) as JObject;

            var resourceSet = GetResourceSetFromQueryResult(bingQueryResult);
            if (resourceSet != null)
            {
                return GetCoordinateFromResourceSet(resourceSet);
            }
            else
            {
                return null;
            }
        }

        private static JToken GetResourceSetFromQueryResult(JObject bingQueryResult)
        {
            const string resourceSetsPropertyName = "resourceSets";

            if (bingQueryResult.HasAnyChildren(resourceSetsPropertyName))
            {
                return bingQueryResult[resourceSetsPropertyName][0];
            }
            return null;
        }

        private static Coordinate GetCoordinateFromResourceSet(JToken resourceSet)
        {
            const string resourcesPropertyName = "resources";

            if (resourceSet.HasAnyChildren(resourcesPropertyName))
            {
                int returnedCoordinateCount = GetReturnedCoordinateCount(resourceSet);
                var resources = resourceSet[resourcesPropertyName];

                // Only get coordinate if confidence level is High, because the coordinate
                // is exactly at the given address. If confidence level is Medium, the coordinate
                // is at the center of the city.
                return GetHighConfidenceCoordinate(resources, returnedCoordinateCount);
            }

            return null;
        }

        private static Coordinate GetHighConfidenceCoordinate(JToken resources, int resourceCount)
        {
            for (int i = 0; i < resourceCount; i++)
            {
                var resource = resources[i];

                if (IsHighConfidenceResource(resource))
                {
                    return GetCoordinateFromResource(resource);
                }
            }
            return null;
        }

        private static Coordinate GetCoordinateFromResource(JToken resource)
        {
            var coordinates = resource["point"]["coordinates"];
            var latitude = coordinates[0].Value<double>();
            var longitude = coordinates[1].Value<double>();

            return new Coordinate(latitude, longitude);
        }

        private static bool IsHighConfidenceResource(JToken resource)
        {
            return resource["confidence"].Value<string>() == "High";
        }

        private static int GetReturnedCoordinateCount(JToken resourceSet)
        {
            return resourceSet["estimatedTotal"].Value<int>();
        }

        private void LogReturnedCoordinateForDebug(string address, Coordinate coordinate)
        {
#if DEBUG
            if (coordinate != null)
                Debug.WriteLine("BingMapsUtil: address = " + address + " | coordinate = " + coordinate.Latitude + ", " + coordinate.Longitude);
            else
                Debug.WriteLine("BingMapsUtil: address = " + address + " | no coordinate");
#endif
        }

        private void LogGeocodeQueryForDebug(string address, string bingGeocodeQuery)
        {
#if DEBUG
            Debug.WriteLine("BingMapsUtil: address = " + address + " | query = " + bingGeocodeQuery);
#endif
        }

        private void BreakDebuggerOnError()
        {
#if DEBUG
            if (Debugger.IsAttached)
                Debugger.Break();
#endif
        }
    }
}
