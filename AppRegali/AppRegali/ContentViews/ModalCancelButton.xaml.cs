using AppRegali.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AppRegali.ContentViews
{
    public partial class ModalCancelButton : ContentView
    {
        public ModalCancelButton()
        {
            InitializeComponent();
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch
            {
                try
                {
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
