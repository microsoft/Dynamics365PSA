using Common.Model.Map;
using Common.Utilities.Map;
using Common.Utilities.Network;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Utilities.Tests.Map
{
    [TestClass]
    public class BingMapsUtilTest
    {
        INetwork mockNetwork;

        string address;
        double expectedLatitude;
        double expectedLongitude;

        string responseHeader = "{\"authenticationResultCode\":\"ValidCredentials\",\"brandLogoUri\":\"http:\\/\\/dev.virtualearth.net\\/Branding\\/logo_powered_by.png\",\"copyright\":\"Copyright © 2015 Microsoft and its suppliers. All rights reserved. This API cannot be accessed and the content and any results may not be used, reproduced or transmitted in any manner without express written permission from Microsoft Corporation.\",";

        BingMapsUtil bingMapsUtil;

        [TestInitialize]
        public void Initialize()
        {
            mockNetwork = Substitute.For<INetwork>();

            address = "One Microsoft Way, Redmond, WA 98052, United States";
            expectedLatitude = 47.640049383044;
            expectedLongitude = -122.129796892404;

            var bingMapsKey = "BingMapsKey";
            bingMapsUtil = new BingMapsUtil(bingMapsKey, mockNetwork);
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldParseCoordinateCorrectly()
        {
            // This response contains only 1 result with High confidence
            var mockResponseContent = responseHeader + "\"resourceSets\":[{\"estimatedTotal\":1,\"resources\":[{\"__type\":\"Location:http:\\/\\/schemas.microsoft.com\\/search\\/local\\/ws\\/rest\\/v1\",\"bbox\":[47.636186665473566,-122.13744013372656,47.643912100614919,-122.12215365108256],\"name\":\"1 Microsoft Way, Redmond, WA 98052\",\"point\":{\"type\":\"Point\",\"coordinates\":[" + expectedLatitude + "," + expectedLongitude + "]},\"address\":{\"addressLine\":\"1 Microsoft Way\",\"adminDistrict\":\"WA\",\"adminDistrict2\":\"King Co.\",\"countryRegion\":\"United States\",\"formattedAddress\":\"1 Microsoft Way, Redmond, WA 98052\",\"locality\":\"Redmond\",\"postalCode\":\"98052\"},\"confidence\":\"High\",\"entityType\":\"Address\",\"geocodePoints\":[{\"type\":\"Point\",\"coordinates\":[47.640049383044243,-122.12979689240456],\"calculationMethod\":\"InterpolationOffset\",\"usageTypes\":[\"Display\"]},{\"type\":\"Point\",\"coordinates\":[47.640068158507347,-122.12985791265965],\"calculationMethod\":\"Interpolation\",\"usageTypes\":[\"Route\"]}],\"matchCodes\":[\"Good\"]}]}],\"statusCode\":200,\"statusDescription\":\"OK\",\"traceId\":\"f1e0f860b3534977baa22ca54818605a|CH10043639|02.00.163.2700|CH1SCH060020225, CH1SCH060051742\"}";

            var coordinate = await GetCoordinateFromAddressAsync_SetupOKResponseContentAndTriggerMethod(mockResponseContent);
            
            coordinate.Latitude.Should().Be(expectedLatitude, "coordinate latitude should be parsed correctly");
            coordinate.Longitude.Should().Be(expectedLongitude, "coordinate longitude should be parsed correctly");
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldReturnNullIfNoHighConfidence()
        {
            // This response contains 2 results with Medium confidence
            var mockResponseContent = responseHeader + "\"resourceSets\":[{\"estimatedTotal\":2,\"resources\":[{\"__type\":\"Location:http:\\/\\/schemas.microsoft.com\\/search\\/local\\/ws\\/rest\\/v1\",\"bbox\":[40.994590759277344,-111.05799865722656,45.013477325439453,-104.05182647705078],\"name\":\"Wyoming\",\"point\":{\"type\":\"Point\",\"coordinates\":[" + expectedLatitude + "," + expectedLongitude + "]},\"address\":{\"adminDistrict\":\"WY\",\"countryRegion\":\"United States\",\"formattedAddress\":\"Wyoming\"},\"confidence\":\"Medium\",\"entityType\":\"AdminDivision1\",\"geocodePoints\":[{\"type\":\"Point\",\"coordinates\":[42.999576568603516,-107.55152130126953],\"calculationMethod\":\"Rooftop\",\"usageTypes\":[\"Display\"]}],\"matchCodes\":[\"Good\",\"UpHierarchy\"]},{\"__type\":\"Location:http:\\/\\/schemas.microsoft.com\\/search\\/local\\/ws\\/rest\\/v1\",\"bbox\":[40.95098876953125,-73.727897644042969,42.0546760559082,-71.787162780761719],\"name\":\"Connecticut\",\"point\":{\"type\":\"Point\",\"coordinates\":[41.573501586914063,-72.7383041381836]},\"address\":{\"adminDistrict\":\"CT\",\"countryRegion\":\"United States\",\"formattedAddress\":\"Connecticut\"},\"confidence\":\"Medium\",\"entityType\":\"AdminDivision1\",\"geocodePoints\":[{\"type\":\"Point\",\"coordinates\":[41.573501586914063,-72.7383041381836],\"calculationMethod\":\"Rooftop\",\"usageTypes\":[\"Display\"]}],\"matchCodes\":[\"Good\",\"UpHierarchy\"]}]}],\"statusCode\":200,\"statusDescription\":\"OK\",\"traceId\":\"7bb94f4d6c81439a9f14a856de5c4a57|CO30276301|02.00.163.2700|CO3SCH010265409, CO3SCH010265304, CO3SCH050520523, CO3SCH010273835, CO3SCH010265210, CO3SCH050581233\"}";

            var coordinate = await GetCoordinateFromAddressAsync_SetupOKResponseContentAndTriggerMethod(mockResponseContent);

            coordinate.Should().BeNull("no coordinate should be returned when there are no High confidence results");
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldOnlyReturnCoordinateWithHighConfidence()
        {
            // This response contains 1 result with Medium confidence and 1 result with High confidence
            var mockResponseContent = responseHeader + "\"resourceSets\":[{\"estimatedTotal\":2,\"resources\":[{\"__type\":\"Location:http:\\/\\/schemas.microsoft.com\\/search\\/local\\/ws\\/rest\\/v1\",\"bbox\":[40.994590759277344,-111.05799865722656,45.013477325439453,-104.05182647705078],\"name\":\"Wyoming\",\"point\":{\"type\":\"Point\",\"coordinates\":[42.999576568603516,-107.55152130126953]},\"address\":{\"adminDistrict\":\"WY\",\"countryRegion\":\"United States\",\"formattedAddress\":\"Wyoming\"},\"confidence\":\"Medium\",\"entityType\":\"AdminDivision1\",\"geocodePoints\":[{\"type\":\"Point\",\"coordinates\":[42.999576568603516,-107.55152130126953],\"calculationMethod\":\"Rooftop\",\"usageTypes\":[\"Display\"]}],\"matchCodes\":[\"Good\",\"UpHierarchy\"]},{\"__type\":\"Location:http:\\/\\/schemas.microsoft.com\\/search\\/local\\/ws\\/rest\\/v1\",\"bbox\":[40.95098876953125,-73.727897644042969,42.0546760559082,-71.787162780761719],\"name\":\"Connecticut\",\"point\":{\"type\":\"Point\",\"coordinates\":[" + expectedLatitude + "," + expectedLongitude + "]},\"address\":{\"adminDistrict\":\"CT\",\"countryRegion\":\"United States\",\"formattedAddress\":\"Connecticut\"},\"confidence\":\"High\",\"entityType\":\"AdminDivision1\",\"geocodePoints\":[{\"type\":\"Point\",\"coordinates\":[41.573501586914063,-72.7383041381836],\"calculationMethod\":\"Rooftop\",\"usageTypes\":[\"Display\"]}],\"matchCodes\":[\"Good\",\"UpHierarchy\"]}]}],\"statusCode\":200,\"statusDescription\":\"OK\",\"traceId\":\"7bb94f4d6c81439a9f14a856de5c4a57|CO30276301|02.00.163.2700|CO3SCH010265409, CO3SCH010265304, CO3SCH050520523, CO3SCH010273835, CO3SCH010265210, CO3SCH050581233\"}";

            var coordinate = await GetCoordinateFromAddressAsync_SetupOKResponseContentAndTriggerMethod(mockResponseContent);

            coordinate.Latitude.Should().Be(expectedLatitude, "coordinate latitude should be extracted from the High confidence result");
            coordinate.Longitude.Should().Be(expectedLongitude, "coordinate longitude should be extracted from the High confidence result");
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldReturnNullIfNoCoordinateFound()
        {
            // This response contain 0 results
            var mockResponseContent = responseHeader + "\"resourceSets\":[{\"estimatedTotal\":0,\"resources\":[]}],\"statusCode\":200,\"statusDescription\":\"OK\",\"traceId\":\"2c1ee1fca2a446d596f3a34c3581c3a4|CO30275936|02.00.174.2300|CO3SCH010264807\"}";

            var coordinate = await GetCoordinateFromAddressAsync_SetupOKResponseContentAndTriggerMethod(mockResponseContent);

            coordinate.Should().BeNull("no coordinate should be returned when the response does not have any result");
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldReturnNullIfNetworkError()
        {
            var mockNetworkResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockNetwork.GetAsyncAsJson(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(mockNetworkResponse));

            var coordinate = await bingMapsUtil.GetCoordinateFromAddressAsync(address);

            coordinate.Should().BeNull("no coordinate should be returned when the response is not 200 OK");
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldReturnNullIfNetworkCrashes()
        {
            mockNetwork.GetAsyncAsJson(Arg.Any<string>(), Arg.Any<string>())
                .Returns(p => { throw new System.Exception(); });

            var coordinate = await bingMapsUtil.GetCoordinateFromAddressAsync(address);
            
            coordinate.Should().BeNull("no coordinate should be returned when there is exception, as this should not crash the app");
        }

        [TestMethod]
        public async Task GetCoordinateFromAddressAsync_ShouldReturnNullForNullAddress()
        {
            var coordinate = await bingMapsUtil.GetCoordinateFromAddressAsync(null);
            coordinate.Should().BeNull("no coordinate should be returned when address is null");
        }

        private async Task<Coordinate> GetCoordinateFromAddressAsync_SetupOKResponseContentAndTriggerMethod(string mockResponseContent)
        {
            var mockNetworkResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockResponseContent)
            };

            mockNetwork.GetAsyncAsJson(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(mockNetworkResponse));

            var coordinate = await bingMapsUtil.GetCoordinateFromAddressAsync(address);
            return coordinate;
        }
    }
}
