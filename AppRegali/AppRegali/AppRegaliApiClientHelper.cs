using Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRegali.Api
{
    public static class ApiHelper
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

        public static async Task SetTokenAsync(string Username, string Password, Uri Endpoint)
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
                    throw new Exception("Si è verificato un errore");
                }
            }
        }

        public static string GetToken()
        {
            return Application.Current.Properties["Access_Token"].ToString() ;
        }

        public static async Task RegisterAsync(string Email, string Password, string ConfermaPassword)
        {
            using (var httpClient = new HttpClient())
            {
                AccountClient accountClient = new AccountClient(httpClient);

                //Creo il modello dei dati per la registrazione
                RegisterBindingModel registerBindingModel = new RegisterBindingModel()
                {
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfermaPassword
                };

                try
                {
                    await accountClient.RegisterAsync(registerBindingModel);
                }
                catch (ApiException ex)
                {
                    throw new Exception("Si è verificato un errore" + ex.StatusCode);
                }
            }
        }
    }
}
