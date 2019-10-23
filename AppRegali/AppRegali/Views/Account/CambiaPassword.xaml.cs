using Api;
using AppRegali.Api;
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
                bool formIsValid = true;

                //Controllo validità della vecchia password.
                if (String.IsNullOrEmpty(entVecchiaPassword.Text))
                {
                    formIsValid = false;
                    lblValidatorEntVecchiaPassword.IsVisible = true;
                }

                //Controllo validità della nuova password.
                if (String.IsNullOrEmpty(entNuovaPassword.Text) || !Regex.IsMatch(entNuovaPassword.Text, Utility.Utility.PasswordRegex))
                {
                    formIsValid = false;
                    lblValidatorEntPassword.IsVisible = true;
                }

                //Controllo che le due password siano uguali.
                if (String.IsNullOrEmpty(entConfermaPassword.Text) || !(entNuovaPassword.Text == entConfermaPassword.Text))
                {
                    formIsValid = false;
                    lblValidatorEntConfermaPassword.IsVisible = true;
                }

                //Se la form è valida proseguo con la registrazione.
                if (formIsValid)
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

                    await accountClient.ChangePasswordAsync(changePasswordBindingModel);
                }
            }
            catch (ApiException ex)
            {
                //Se sono qui non ho l'accesso, quindi la password è sbagliata.
                lblErrore.IsVisible = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}