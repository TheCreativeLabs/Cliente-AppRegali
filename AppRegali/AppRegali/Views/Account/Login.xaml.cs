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

        private async void btnAccedi_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                bool formIsValid = true;

                if (String.IsNullOrEmpty(entUsername.Text))
                {
                    formIsValid = false;
                    lblValidatorUsername.IsVisible = true;
                }

                if (String.IsNullOrEmpty(entPassword.Text))
                {
                    formIsValid = false;
                    lblValidatorPassword.IsVisible = true;
                }

                if (formIsValid)
                {
                    await ApiHelper.SetTokenAsync(entUsername.Text, entPassword.Text, new Uri("https://www.appregaliapitest.com/Token"));
                    Application.Current.MainPage = new MainPage();
                }
            }
            catch (Exception ex)
            {
               //Gestione errore;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PasswordDimenticata());
        }

        private void TapGestureRecognizer_lblRegistrati(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Registrazione());
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
    }
}