using System;
using System.Collections.Generic;
using AppRegali.Views.DesideriView;
using Xamarin.Forms;

namespace AppRegali.Views
{
    public partial class AddPage : ContentPage
    {
        public AddPage()
        {
            InitializeComponent();
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async void FrameNuovoDesiderioRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DesideriInserisci());
        }

        async void FrameNuovoEventoRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new EventoInserisci());
        }
    }
}
