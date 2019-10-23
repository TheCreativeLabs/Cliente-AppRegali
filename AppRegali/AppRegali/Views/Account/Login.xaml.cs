using Api;
using AppRegali.Api;
using AppRegali.Views.Account;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();

            //Se ho il token allora vado direttamente alla home
            if (ApiHelper.GetToken() != null)
            {
                Application.Current.MainPage = new MainPage();
            }
        }

        private async void btnAccedi_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                bool formIsValid = true;

                //Controllo che username e password siano valorizzati.
                if (String.IsNullOrEmpty(entUsername.Text))
                {
                    formIsValid = false;
                    //lblValidatorUsername.IsVisible = true;
                }

                if (String.IsNullOrEmpty(entPassword.Text))
                {
                    formIsValid = false;
                    //lblValidatorPassword.IsVisible = true;
                }

                if (formIsValid)
                {
                    await ApiHelper.SetTokenAsync(entUsername.Text, entPassword.Text, new Uri("https://www.appregaliapitest.com/Token"));
                    Application.Current.MainPage = new MainPage();
                }
            }
            catch (ApplicationException ex)
            {
                //Se sono qui significa che non ho i diritti per accedere.
                //lblValidazioneLogin.IsVisi/*b*/le = true;

                await DisplayAlert("Attenzione", "L'indirizzo email o la password non sono validi.", "OK");
            }
            catch (Exception ex)
            {
               //In questo caso se 
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new PasswordDimenticata()));

            //Navigation.PushAsync(new PasswordDimenticata());
        }

        private async void TapGestureRecognizer_lblRegistrati(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Registrazione()));

            //Navigation.PushAsync(new Registrazione());
        }

        private void btnAccediFacebook_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new Account.FacebookLogin());
            }
            catch (Exception ex)
            {
                //Gestione errore;
            }
        }

        private void ent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Controllo che username e password siano valorizzati.
                if (!(String.IsNullOrEmpty(entUsername.Text)) && !(String.IsNullOrEmpty(entPassword.Text)))
                {
                    btnAccedi.IsEnabled = true;
                }
                else
                {
                    btnAccedi.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}