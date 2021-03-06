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
                    await DisplayAlert("Attenzione", "Inserire la email", "OK");
                }
                else
                {
                    HttpClient httpClient = new HttpClient();
                    AccountClient accountClient = new AccountClient(httpClient);
                    await accountClient.RestorePasswordAsync(entEmail.Text);

                    await DisplayAlert("Completato", "Ti abbiamo inviato una email contentente la tua nuova password", "OK");

                    await Navigation.PopModalAsync();

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

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void entEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Controllo che username e password siano valorizzati.
            if (!(String.IsNullOrEmpty(entEmail.Text)))
            {
                btnProsegui.IsEnabled = true;
            }
            else
            {
                btnProsegui.IsEnabled = false;
            }
        }
    }
}