using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRegali.Api
{
    public class ApiHelper
    {
        public class BearerToken
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty(".issued")]
            public string Issued { get; set; }

            [JsonProperty(".expires")]
            public string Expires { get; set; }
        }

        public async Task SetTokenAsync(string Username, string Password, Uri Endpoint)
        {
            using (var httpClient = new HttpClient())
            {
                var tokenRequest =
                    new List<KeyValuePair<string, string>>
                        {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", Username),
                        new KeyValuePair<string, string>("password", Password)
                        };

                HttpContent encodedRequest = new FormUrlEncodedContent(tokenRequest);

                HttpResponseMessage response = await httpClient.PostAsync(Endpoint, encodedRequest);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var o = response.Content.ReadAsStringAsync().Result;
                    BearerToken token2 = JsonConvert.DeserializeObject<BearerToken>(o);

                    Application.Current.Properties["Access_Token"] = token2.AccessToken;
                }
                else
                {
                    throw new Exception("Non autorizzato.");
                }
            }
        }

        public string GetToken()
        {
            return Application.Current.Properties["Access_Token"].ToString() ;
        }
    }
}
