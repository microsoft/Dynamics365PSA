using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Utilities.Office
{
    public class Delve
    {
        private static Delve current;
        public static Delve Current
        {
            get
            {
                if (current == null)
                {
                    current = new Delve();
                }
                return current;
            }
        }

        private Authentication.Authentication.AuthenticationResult authResult;
        private string serverUrl;

        // Query the Office Graph on SharePoint
        // Using action 1019, which is to search for WorkingWith - People whom the actor communicates with or works with frequently
        // Sort by the weight of the edge, which is the relevant score between ME and the other people
        // https://msdn.microsoft.com/office/office365/HowTo/query-Office-graph-using-gql-with-search-rest-api
        private const string usersWorkingWithQuery = "/_api/search/query?Querytext='*'&Properties='GraphQuery:ACTOR(ME\\,action\\:1019),GraphRankingModel:{\"features\"\\:[{\"function\"\\:\"EdgeWeight\"}]}'&RankingModelId='0c77ded8-c3ef-466d-929d-905670ea1d72'&SelectProperties='Title,Path,OriginalPath,Edges,PictureThumbnailURL'&RowLimit=100";
        private const string userProfile_PictureURLQuery = "/_api/sp.userprofiles.peoplemanager/getuserprofilepropertyfor(accountname=@v, propertyname='PictureURL')?@v=%27i%3A0%23%2Ef%7Cmembership%7C{0}%27";

        public bool? IsSharePointAvailable { get; private set; }

        private Delve() { }

        private async Task AuthenticateAsync()
        {
            if (IsSharePointAvailable == null)
            {
                serverUrl = await Office365.Current.GetServiceEndpointAsync(Office365Service.SharePoint);

                if (serverUrl != null)
                {
                    authResult = await Authentication.Authentication.Current.AcquireTokenByRefreshToken(serverUrl);
                    IsSharePointAvailable = (authResult.Status == Authentication.Authentication.AuthenticationStatus.Success);
                }
                else
                {
                    IsSharePointAvailable = false;
                }
            }
        }

        public async Task<List<DelveUsersWorkingWithResult>> GetOfficeGraph_UsersWorkingWith()
        {
            if (authResult == null)
            {
                await AuthenticateAsync();
            }

            if (IsSharePointAvailable == true)
            {
                try
                {
                    string query = serverUrl.TrimEnd('/') + usersWorkingWithQuery;

                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await httpClient.GetAsync(query);

                    if (response.IsSuccessStatusCode)
                    {
                        var usersWorkingWithJson = await response.Content.ReadAsStringAsync();
                        return ParseDelveUsersWorkingWith(usersWorkingWithJson);
                    }
                    else
                    {
                        return new List<DelveUsersWorkingWithResult>();
                    }
                }
                // This exception occurs on Android when there is no network connection
                catch (System.Net.WebException)
                {
                    return new List<DelveUsersWorkingWithResult>();
                }
            }
            else
            {
                return new List<DelveUsersWorkingWithResult>();
            }
        }

        public bool LogOut()
        {
            authResult = null;
            serverUrl = null;
            IsSharePointAvailable = null;
            return true;
        }

        private List<DelveUsersWorkingWithResult> ParseDelveUsersWorkingWith(string usersWorkingWithJson)
        {
            var usersWorkingWithObj = JObject.Parse(usersWorkingWithJson);

            var primaryQueryResult = usersWorkingWithObj["PrimaryQueryResult"];
            if (primaryQueryResult != null && primaryQueryResult.HasValues && primaryQueryResult["RelevantResults"] != null)
            {
                var resultRows = primaryQueryResult["RelevantResults"]["Table"]["Rows"].Select(row => row["Cells"]).ToList();
                var usersWorkingWith = new List<DelveUsersWorkingWithResult>();

                foreach (var resultRow in resultRows)
                {
                    var usersWorkingWithResult = new DelveUsersWorkingWithResult();

                    foreach (var cell in resultRow)
                    {
                        var key = cell["Key"].ToString();
                        var value = cell["Value"].ToString();

                        if (!string.IsNullOrEmpty(key))
                        {
                            switch (key)
                            {
                                case "Path":
                                    {
                                        var uri = new Uri(value);

                                        var upn = Regex.Match(uri.Query, @"%7C([^%]*%40.*)$", RegexOptions.IgnoreCase).Groups[1].Value;
                                        upn = upn.Replace("%40", "@").Replace("%2E", ".");

                                        var alias = Regex.Match(upn, @"(.*)@.*$", RegexOptions.IgnoreCase).Groups[1].Value;

                                        usersWorkingWithResult.Upn = upn;
                                        usersWorkingWithResult.Alias = alias;
                                        break;
                                    }
                                case "Edges":
                                    {
                                        var edges = JArray.Parse(value);
                                        var edge = edges[0];
                                        if (edge != null)
                                        {
                                            usersWorkingWithResult.Weight = int.Parse(edge["Properties"]["Weight"].ToString());
                                        }
                                        break;
                                    }
                                case "Title":
                                    {
                                        usersWorkingWithResult.Title = value;
                                        break;
                                    }
                            }
                        }
                    }

                    usersWorkingWith.Add(usersWorkingWithResult);
                }

                return usersWorkingWith;
            }
            else
            {
                return new List<DelveUsersWorkingWithResult>();
            }
        }
    }

    public class DelveUsersWorkingWithResult
    {
        public string Title { get; set; }
        public string Upn { get; set; }
        public string Alias { get; set; }
        public string PictureThumbnailUrl { get; set; }
        public int Weight { get; set; }
    }
}
