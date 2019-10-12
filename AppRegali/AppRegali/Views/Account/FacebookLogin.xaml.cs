using Api;
using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Account
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FacebookLogin : ContentPage
    {
        public FacebookLogin()
        {
            InitializeComponent();

            AccountClient accountClient = new AccountClient(new System.Net.Http.HttpClient());
            ExternalLoginViewModel externalLoginViewModel = accountClient.GetExternalLoginsAsync("/", true).Result.First();

            var apiRequest = "https://localhost:44373" + externalLoginViewModel.Url;
                //"https://www.facebook.com/v4.0/dialog/oauth?client_id="
                //+ "971997736480952"
                //+ "&display=popup&response_type=token&redirect_uri=https://www.facebook.com/connect/login_success.html";

            //var apiRequest =
            //    "https://www.facebook.com/v4.0/dialog/oauth?client_id="
            //    + "971997736480952"
            //    + "&display=popup&response_type=token&redirect_uri=https://www.facebook.com/connect/login_success.html";

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
                var vm = BindingContext as FacebookViewModel;

                await vm.SetFacebookUserProfileAsync(accessToken);

                Content = MainStackLayout;
            }
        }

        private string ExtractAccessTokenFromUrl(string url)
        {
            if (url.Contains("access_token") && url.Contains("&expires_in="))
            {
                var at = url.Replace("https://www.facebook.com/connect/login_success.html#access_token=", "");

                if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
                {
                    at = url.Replace("http://www.facebook.com/connect/login_success.html#access_token=", "");
                }

                var accessToken = at.Remove(at.IndexOf("&data_access_expiration_time="));

                return accessToken;
            }

            return string.Empty;
        }
    }
}