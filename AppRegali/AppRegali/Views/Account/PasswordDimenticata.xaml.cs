using Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordDimenticata : ContentPage
    {
        public PasswordDimenticata()
        {
            InitializeComponent();
        }

        private async void btnProsegui_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(entEmail.Text) || !Regex.IsMatch(entEmail.Text, Utility.Utility.EmailRegex))
                {
                    lblValidatorEntEmail.IsVisible = true;
                }
                else
                {
                    HttpClient httpClient = new HttpClient();
                    AccountClient accountClient = new AccountClient(httpClient);
                    await accountClient.RestorePasswordAsync(entEmail.Text);

                    stkFormRestore.IsVisible = false;
                    stkRestoreAvvenuto.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void btnLogin_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Login());
        }
    }
}