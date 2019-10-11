﻿using Api;
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
    public partial class Registrazione : ContentPage
    {
        public Registrazione()
        {
            InitializeComponent();
        }

        private async void btnRegistrati_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool formIsValid = true;

                //Controllo validità della mail.
                if (!Regex.IsMatch(entEmail.Text, Utility.Utility.EmailRegex))
                {
                    formIsValid = false;
                    lblValidatorEntEmail.IsVisible = true;
                }

                //Controllo validità della password.
                if (!Regex.IsMatch(entPassword.Text, Utility.Utility.PasswordRegex))
                {
                    formIsValid = false;
                    lblValidatorEntPassword.IsVisible = true;
                }

                //Controllo che le due password siano uguali.
                if (!(entPassword.Text == entConfermaPassword.Text))
                {
                    formIsValid = false;
                    lblValidatorEntConfermaPassword.IsVisible = true;
                }

                //Se la form è valida proseguo con la registrazione.
                if (formIsValid)
                {
                    await AppRegali.Api.ApiHelper.RegisterAsync(entEmail.Text, entPassword.Text, entConfermaPassword.Text);
                    stkFormRegistrazione.IsVisible = false;
                    stkRegistrazioneAvvenuta.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //TODO Gestire l'errore navigando alla pagina specifica.
            }
        }

        private void btnLogin_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Login());
        }
    }
}