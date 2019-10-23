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

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventoInserisci : ContentPage
    {
        CategorieViewModel viewModel;

        public EventoInserisci()
        {
            InitializeComponent();

            BindingContext = viewModel = new CategorieViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiHelper.GetToken());
            EventoClient eventoClient = new EventoClient(httpClient);

            await Navigation.PopModalAsync();
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        //private async void entCategoria_Focused(object sender, FocusEventArgs e)
        //{
        //    entCategoria.Unfocus();
        //    await Navigation.PushAsync(new SelezioneCategoria());
        //}

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
            entCategoria.Text = ((EventoCategoria)pkCategoria.SelectedItem).Descrizione;
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                imgTest.Source = ImageSource.FromStream(() => stream);
            }

            (sender as Button).IsEnabled = true;
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