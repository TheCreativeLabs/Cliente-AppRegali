using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppRegali.Models;
using AppRegali.ViewModels;
using Api;
using AppRegali.Api;
using System.Linq;

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
            viewModel.Item.Regali = null;
            BindingContext = viewModel;

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<RegaloDettaglio, string>(this, "RefreshListaRegaliDettaglio", async (sender, arg) =>
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    viewModel.Item.Regali = null;
                }
            });

            if (viewModel.Item.Regali == null || !viewModel.Item.Regali.Any())
            {
                RegaliActivityIndicator.IsRunning = true;
                RegaliActivityIndicator.IsVisible = true;

                EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
                var item = await eventoClient.GetEventoByIdAsync(new Guid(viewModel.Item.Id));
                viewModel.Item.CodiceCategoriaEvento = item.CodiceCategoriaEvento;
                FrameTitle.IsVisible = true;
                viewModel.Item.Regali = item.Regali;
            }

            RegaliDettaglioListView.ItemsSource = viewModel.Item.Regali;

            //if (eventoDettaglio.Regali != null)
            //{
            //    // RegaliDettaglioListView.HeightRequest = ((140) * eventoDettaglio.Regali.Count) + 70;
            //}

            RegaliActivityIndicator.IsRunning = false;
            RegaliActivityIndicator.IsVisible = false;
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as RegaloDtoOutput;

            if (item == null || item.Id == null)
                return;

            await Navigation.PushModalAsync(new RegaloDettaglio(item));

            RegaliDettaglioListView.SelectedItem = null;
        }   
    }
}