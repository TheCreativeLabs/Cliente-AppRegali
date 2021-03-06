﻿using Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AppRegali.Api
{
    public static class ApiHelper
    {

        public enum LoginProvider
        {
            Facebook = 0,
            Google = 1,
            Email = 2
        }

        public const string AccessTokenKey = "Access_Token";
        public const string IsFacebookLoginKey = "Is_Facebook_Login";
        public const string ProviderKey = "Provider_Key";
        public const string UserInfoKey = "UserInfo_Key";
        public const string CategorieKey = "Categorie_Key";


        public class BearerToken
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty(".issued")]
            public string Issued { get; set; }

            [JsonProperty(".expires")]
            public string Expires { get; set; }
        }

        /// <summary>
        /// Setta il token
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static async Task SetTokenAsync(string Username, string Password)
        {
            using (var httpClient = new HttpClient())
            {
                Uri Endpoint = new Uri($"{AppSetting.ApiEndpoint}Token");

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
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    SetToken(jsonResponse);
                }
                else
                {
                    //Se arrivo qui allora email e password sono sbagliati.
                    throw new ApplicationException("Password o Email errati.");
                }
            }
        }


        public static async Task<string> GetRefreshToken(string RefreshToken)
        {
            using (var httpClient = new HttpClient())
            {
                Uri Endpoint = new Uri($"{AppSetting.ApiEndpoint}Token");

                var tokenRequest =
                    new List<KeyValuePair<string, string>>
                        {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", RefreshToken)
                        };

                HttpContent encodedRequest = new FormUrlEncodedContent(tokenRequest);

                HttpResponseMessage response = await httpClient.PostAsync(Endpoint, encodedRequest);

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    SetToken(jsonResponse);

                    BearerToken token = JsonConvert.DeserializeObject<BearerToken>(jsonResponse);

                    return token.AccessToken;
                }
                else
                {
                    //Se arrivo qui allora email e password sono sbagliati.
                    throw new ApplicationException("Password o Email errati.");
                }
            }
        }


        public static void SetToken(string AccessToken)
        {
            Preferences.Set(AccessTokenKey, AccessToken);
        }

        // Ottiene il Token
        public async static Task<string> GetToken()
        {
            string accessToken = null;

            var bearerToken = Preferences.Get(AccessTokenKey, null);

            if (Api.ApiHelper.LoginProvider.Facebook.Equals(GetProvider())) //provider facebook: non serve fare refresh token. ho direttamente il token in bearerToken
            {
                accessToken = bearerToken;
            } else
            { //registrazione con mail: se necessario si fa refreshToken
                if (bearerToken != null && !string.IsNullOrEmpty(bearerToken))
                {
                    BearerToken token = JsonConvert.DeserializeObject<BearerToken>(bearerToken);

                    var expireDate = DateTime.Parse(token.Expires);
                    //DateTime.ParseExact(
                    //            token.Expires,
                    //            "ddd MMM dd yyyy HH:mm:ss 'GMT'",
                    //            CultureInfo.InvariantCulture);
                    if (expireDate < DateTime.Now)
                    {
                        accessToken = await GetRefreshToken(token.RefreshToken);
                    }
                    else
                    {
                        accessToken = token.AccessToken;
                    }
                }
            }
            


            return accessToken;
        }

        /// <summary>
        /// Rimuove il token.
        /// </summary>
        /// <returns></returns>
        public static async void DeleteToken()
        {
            Preferences.Remove(AccessTokenKey);
        }


        /// <summary>
        /// Restituisce il Client da usare per le chiamate all'api
        /// </summary>
        /// <returns></returns>
        public async static Task<HttpClient> GetApiClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await Api.ApiHelper.GetToken());
            return httpClient;
        }

        /// <summary>
        /// Metodo per registrarsi
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="ConfermaPassword"></param>
        /// <returns></returns>
        public static async Task RegisterAsync(string Email, string Password, string ConfermaPassword)
        {
            using (var httpClient = new HttpClient())
            {
                AccountClient accountClient = new AccountClient(httpClient);

                //Creo il modello dei dati per la registrazione
                RegisterUserBindingModel registerBindingModel = new RegisterUserBindingModel()
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


        public static async Task<UserInfoDto> GetUserInfo()
        {
            UserInfoDto userInfo = null;

            if (Preferences.Get(UserInfoKey, null) != null)
            {
                userInfo = JsonConvert.DeserializeObject<UserInfoDto>(Preferences.Get(UserInfoKey, null));
            }

            if (userInfo == null)
            {
                AmiciClient amiciClient = new AmiciClient(await ApiHelper.GetApiClient());
                userInfo = await amiciClient.GetCurrentUserInfoAsync();
                Preferences.Set(UserInfoKey, JsonConvert.SerializeObject(userInfo));
            }

            return userInfo;
        }

        public static void RemoveUserInfo()
        {
            Preferences.Remove(UserInfoKey);
        }

        /// <summary>
        /// Rimuove tutte le setting
        /// </summary>
        public static void RemoveSettings()
        {
            Preferences.Clear();
        }


        /// <summary>
        /// Salva nella cache se l'utente si è loggato con facebook, google o email.
        /// </summary>
        /// <param name="Provider"></param>
        public static async void SetProvider(LoginProvider Provider)
        {
            Preferences.Set(ProviderKey, Provider.ToString());

        }

        /// <summary>
        /// Restituisce se l'utente si è loggato con facebook,google o email.
        /// </summary>
        /// <returns></returns>
        public static LoginProvider GetProvider()
        {
            LoginProvider provider = LoginProvider.Email;

            if (Preferences.Get(ProviderKey, null) != null)
            {
                Enum.TryParse(Preferences.Get(ProviderKey, null), out provider);
            }

            return provider;
        }



        /// <summary>
        /// Rimuove il provider.
        /// </summary>
        /// <returns></returns>
        public static async void RemoveProvider()
        {

            Preferences.Remove(ProviderKey);

        }

        /// <summary>
        /// Restituisce se l'utente si è loggato con facebook,google o email.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<EventoCategoria>> GetCategorie()
        {
            List<EventoCategoria> listCategorie = new List<EventoCategoria>();


            if (Preferences.Get(CategorieKey, null) != null)
            {
              listCategorie = JsonConvert.DeserializeObject<List<EventoCategoria>>(Preferences.Get(CategorieKey, null));
            }

            if (!listCategorie.Any())
            {
                EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
                ICollection<EventoCategoria> categorie = await eventoClient.GetLookupEventoCategoriaAsync();
                listCategorie = categorie.ToList();
                Preferences.Set(CategorieKey, JsonConvert.SerializeObject(listCategorie));
            }

            return listCategorie;
        }
    }
}
