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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Se ho il token allora vado direttamente alla home
            if (ApiHelper.GetToken() != null)
            {
                //Application.Current.MainPage = new MainPage();
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
                }

                if (String.IsNullOrEmpty(entPassword.Text))
                {
                    formIsValid = false;
                }

                //Se la form è valida allora setto il token
                if (formIsValid)
                {
                    await ApiHelper.SetTokenAsync(entUsername.Text, entPassword.Text);
                    Application.Current.MainPage = new MainPage();
                }
            }
            catch (ApplicationException Ex)
            {
                //Se sono qui significa che non ho i diritti per accedere.
                await DisplayAlert("Attenzione", "L'indirizzo email o la password non sono validi.", "OK");
            }
            catch (Exception Ex)
            {
                //Navigo alla pagina d'errore.
                //await Navigation.PushAsync(new ErrorPage());
                Application.Current.MainPage = new ErrorPage();

            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new NavigationPage(new PasswordDimenticata()));

            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();
            }
        }

        private async void TapGestureRecognizer_lblRegistrati(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new NavigationPage(new Registrazione()));
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();

            }
        }

        private async void btnAccediFacebook_Clicked(object sender, EventArgs e)
        {
            try
            {
                Application.Current.MainPage = new Account.FacebookLogin();
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();

            }
        }

        private async void ent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                btnAccedi.IsEnabled = false;

                //Controllo che username e password siano valorizzati, se lo sono abilito il pulsante.
                if (!(String.IsNullOrEmpty(entUsername.Text)) && !(String.IsNullOrEmpty(entPassword.Text)))
                {
                    btnAccedi.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();

            }
        }
    }
}