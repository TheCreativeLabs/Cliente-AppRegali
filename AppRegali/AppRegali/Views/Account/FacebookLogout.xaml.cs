using Api;
using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Account
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FacebookLogout : ContentPage
    {
        public FacebookLogout()
        {
            InitializeComponent();

            Title = "Facebook Profile";
            BackgroundColor = Color.White;

            var apiRequest =
                "https://www.facebook.com/dialog/oauth?client_id="
                + "971997736480952"
                + "&display=popup&response_type=token&redirect_uri=https://www.appregaliapitest.com/";

            var webView = new WebView
            {
                Source = apiRequest,
                HeightRequest = 1
            };

            webView.Navigated += WebViewOnNavigated;

            Content = webView;
        }


        private async void WebViewOnNavigated(object sender, WebNavigatedEventArgs e)
        {
            var accessToken = ExtractAccessTokenFromUrl(e.Url);

            if (accessToken != "")
            {

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());

                AccountClient accountClient = new AccountClient(httpClient);

                ManageInfoViewModel userInfo = await accountClient.GetManageInfoAsync("/",true);

                string providerKey = ((UserLoginInfoViewModel)userInfo.Logins.First()).ProviderKey;

                var requestUrl =
                $"https://graph.facebook.com/v2.7/{providerKey}/permissions/email?&access_token="
                + accessToken;

                var httpClient2 = new HttpClient();

                var userJson = await httpClient2.DeleteAsync(requestUrl);

                await accountClient.LogoutAsync();

                Application.Current.MainPage = new NavigationPage(new Login.Login());
            }
        }
        private string ExtractAccessTokenFromUrl(string url)
        {
            if (url.Contains("access_token") && url.Contains("&expires_in="))
            {
                var at = url.Replace("https://www.appregaliapitest.com/?#access_token=", "");

                var accessToken = at.Remove(at.IndexOf("&data_access_expiration_time="));

                return accessToken;
            }

            return string.Empty;
        }
    }
}