using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppRegali.Models;
using AppRegali.ViewModels;
using Api;
using AppRegali.Api;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventoDettaglio : ContentPage
    {
        EventoDetailViewModel viewModel;

        public EventoDettaglio(EventoDtoOutput evento)
        {
            InitializeComponent();

            EventoDetailViewModel eventoDetailViewModel = new EventoDetailViewModel();
            eventoDetailViewModel.Item = evento;
            viewModel = eventoDetailViewModel;
            BindingContext = viewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());
            EventoDtoOutput eventoDettaglio = await eventoClient.GetEventoByIdAsync(new Guid(viewModel.Item.Id));
            RegaliDettaglioListView.ItemsSource = eventoDettaglio.Regali;
            if(eventoDettaglio.Regali != null)
            {
                RegaliDettaglioListView.HeightRequest = ((140) * eventoDettaglio.Regali.Count) + 70;
            }
        }
    }
}