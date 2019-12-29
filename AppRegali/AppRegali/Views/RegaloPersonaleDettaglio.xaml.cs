using System;
using System.Collections.Generic;
using System.Linq;
using Api;
using AppRegali.Api;
using Xamarin.Forms;

namespace AppRegali.Views
{
    public partial class RegaloPersonaleDettaglio : ContentPage
    {
        RegaloDtoOutput Regalo;

        public RegaloPersonaleDettaglio(RegaloDtoOutput RegaloParam)
        {
            InitializeComponent();

            Regalo = RegaloParam;
            BindingContext = Regalo;

            if (Regalo != null && Regalo.ImportoCollezionato.HasValue)
                ValueProgress.Progress = Regalo.ImportoCollezionato.Value / Regalo.Prezzo;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            EventoClient eventoClient = new EventoClient(await  ApiHelper.GetApiClient());
            PartecipazioneDtoOutput partecipazione =  await eventoClient.GetPartecipazioniRegaloAsync(new Guid(Regalo.Id));
            lvPartecipanti.ItemsSource = partecipazione.UtentiPartecipanti.ToList();
            if(partecipazione.NumeroAnonimi.HasValue && partecipazione.NumeroAnonimi.Value > 0)
                NumeroPartecipanti.Text = partecipazione.NumeroAnonimi.ToString();
        }
    }
}
