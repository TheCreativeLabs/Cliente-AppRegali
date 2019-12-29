using System;
using System.Collections.Generic;
using Api;
using AppRegali.Api;
using Xamarin.Forms;

namespace AppRegali.Views
{
    public partial class RegaloDettaglio : ContentPage
    {
        RegaloDtoOutput Regalo;

        public RegaloDettaglio(RegaloDtoOutput RegaloParam)
        {
            InitializeComponent();

            Regalo = RegaloParam;
            BindingContext = Regalo;

            if (Regalo != null && Regalo.ImportoCollezionato.HasValue) { 
                ValueProgress.Progress = Regalo.ImportoCollezionato.Value / Regalo.Prezzo;
                if(ValueProgress.Progress == 1)
                {
                    ValueProgress.ProgressColor = (Color)App.Current.Resources["SuccessColor"];
                    btnDonazione.IsEnabled = false;
                }
            }
        }

        private async void BtnDona_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(entDonazione.Text))
            {
                await DisplayAlert("Attenzione", "è necessario inserire una cifra valida", "OK");
                return;
            }

            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
            await eventoClient.InserisciPartecipazioneRegaloAsync(new Guid(Regalo.Id), double.Parse(entDonazione.Text), chkAnonimo.IsChecked);

            await DisplayAlert("Complimenti", "la donazione è avvenuta con successo", "OK");

            MessagingCenter.Send(this, "RefreshLista", "OK");

            await Navigation.PopModalAsync();
        }
    }
}
