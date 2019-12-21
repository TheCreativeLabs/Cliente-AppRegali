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
            await Navigation.PopModalAsync();
        }
    }
}
