using Api;
using AppRegali.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CambiaPassword : ContentPage
    {
        public CambiaPassword()
        {
            InitializeComponent();
        }

        private async void btnProcedi_Clicked(object sender, EventArgs e)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiHelper.GetToken());
                AccountClient accountClient = new AccountClient(httpClient);

                ChangePasswordBindingModel changePasswordBindingModel = new ChangePasswordBindingModel()
                {
                    OldPassword = entVecchiaPassword.Text,
                    NewPassword = entNuovaPassword.Text,
                    ConfirmPassword = entConfermaPassword.Text
                };

                var result = await accountClient.ChangePasswordAsync(changePasswordBindingModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}