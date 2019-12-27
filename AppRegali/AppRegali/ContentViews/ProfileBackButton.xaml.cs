using AppRegali.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AppRegali.ContentViews
{
    public partial class ProfileBackButton : ContentView
    {
        public ProfileBackButton()
        {
            InitializeComponent();
        }

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch 
            {
                try {
                    await Navigation.PopAsync();
                }
                catch
                {
                    await Navigation.PushAsync(new ErrorPage());
                }
            }
        }
    }
}
