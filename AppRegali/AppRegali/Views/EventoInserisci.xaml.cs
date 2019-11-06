using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppRegali.Models;
using System.Net.Http;
using Api;
using AppRegali.Api;
using System.Threading.Tasks;
using AppRegali.ViewModels;
using System.IO;
using DependencyServiceDemos;
using System.Text;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventoInserisci : ContentPage
    {
        CategorieViewModel viewModel;
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();


        byte[] img;
        public EventoInserisci()
        {
            InitializeComponent();

            BindingContext = viewModel = new CategorieViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());

            if (img == null)
            {
                EventoCategoria cat = (EventoCategoria)pkCategoria.SelectedItem;
                img = cat.Immagine;
            }
            //Costruisco l'evento
            EventoDtoInput evento = new EventoDtoInput()
            {
                Titolo = entTitolo.Text,
                Descrizione = edDescrizione.Text,
                IdCategoriaEvento = ((EventoCategoria)pkCategoria.SelectedItem).Id.Value,
                DataEvento = dpDataEvento.Date,
                ImmagineEvento = img
            };

            //Inserisco l'evento
            var eventoInserito = await eventoClient.InserisciEventoAsync(evento);

            //Torno alla pagina di lista
            await Navigation.PopModalAsync();
            //Redirect alla modifica dell'evento appena inserito, in questo modo l'utente può aggiungere regali
            //EventoDtoOutput dettaglioEvento = await eventoClient.GetEventoByIdAsync(new Guid(eventoInserito.Id));
            //await Navigation.PushAsync(new EventoModifica(new EventoDetailViewModel(eventoInserito)));
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void entDataEvento_Focused(object sender, FocusEventArgs e)
        {
            entDataEvento.Unfocus();
            dpDataEvento.Focus();
        }

        private void dpDataEvento_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (dpDataEvento.Date != null)
            {
                entDataEvento.Text = dpDataEvento.Date.ToString("dd/MM/yyyy");
            }
        }

        private void entCategoria_Focused(object sender, FocusEventArgs e)
        {
            entCategoria.Unfocus();
            pkCategoria.Focus();
        }

        private void pkCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            entCategoria.Text = Helpers.TranslateExtension.ResMgr.Value.GetString(((EventoCategoria)pkCategoria.SelectedItem).Codice, translate.ci);
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                imgTest.Source = ImageSource.FromStream(() => stream);

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    img = memoryStream.ToArray();
                }
            }

            (sender as Button).IsEnabled = true;
        }

        async void AddRegalo_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new RegaloInserisci(new RegaloDetailViewModel())));
        }

        private void ent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Controllo che username e password siano valorizzati.
                if (!(String.IsNullOrEmpty(entTitolo.Text)) && !(String.IsNullOrEmpty(edDescrizione.Text))
                    && !(String.IsNullOrEmpty(entCategoria.Text)) && !(String.IsNullOrEmpty(entDataEvento.Text)))
                {
                    btnSalva.IsEnabled = true;
                }
                else
                {
                    btnSalva.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}