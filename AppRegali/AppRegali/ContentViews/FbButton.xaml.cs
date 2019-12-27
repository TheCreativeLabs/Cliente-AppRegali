using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AppRegali.ContentViews
{
    public partial class FbButton : ContentView
    {
        public FbButton()
        {
            InitializeComponent();
        }

        private async void btnAccediFacebook_Clicked(object sender, EventArgs e)
        {
            bool answer = await App.Current.MainPage.DisplayAlert("L'app vuole utilizzare Facebook.com per accedere", "Questo permette all'app e al sito web di accedere alle tue informazioni.", "Si", "No");
            if (answer)
            {
                await Navigation.PushAsync(new NavigationPage(new AppRegali.Views.Account.FacebookLogin()));
            }
        }
    }
}
