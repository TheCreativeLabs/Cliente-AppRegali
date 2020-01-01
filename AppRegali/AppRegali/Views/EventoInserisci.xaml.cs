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
using Plugin.Media;
using Plugin.Media.Abstractions;
using AppRegali.Utility;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventoInserisci : ContentPage
    {
        CategorieViewModel viewModel;
        //static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();


        byte[] img;
        public EventoInserisci()
        {
            InitializeComponent();

            BindingContext = viewModel = new CategorieViewModel();
                  
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            //fixme loading on 
            LoadingEventoInserirsci.IsRunning = true;
            LoadingEventoInserirsci.IsVisible = true;
            if (viewModel.Items.Count == 0)
            {
                //viewModel.LoadItemsCommand.Execute(null);
                await viewModel.LoadCategorie();
            }

            LoadingEventoInserirsci.IsRunning = false;
            LoadingEventoInserirsci.IsVisible = false;
            StackContent.IsVisible = true;
            //fixme loading off
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            EventoInserisciActivityIndicator.IsVisible = true;

            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());

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
            EventoDtoOutput eventoInserito = await eventoClient.InserisciEventoAsync(evento);

            MessagingCenter.Send(this, "RefreshListaEventiPersonaliInsert", "OK");
            EventoInserisciActivityIndicator.IsVisible = false;

            //Torno alla pagina di lista
            await Navigation.PopModalAsync();
            //Redirect alla modifica dell'evento appena inserito, in questo modo l'utente può aggiungere regali
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
            entCategoria.Text = Helpers.TranslateExtension.ResMgr.Value.GetString(((EventoCategoria)pkCategoria.SelectedItem).Codice, CurrentCulture.Ci);
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

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            PickPhoto();

            (sender as Button).IsEnabled = true;
        }

        async void PickPhoto()
        {
            await CrossMedia.Current.Initialize();

            MediaFile foto = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (foto == null)
                return;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                foto.GetStream().CopyTo(memoryStream);

                img = memoryStream.ToArray();
                imgTest.Source = ImageSource.FromStream(() => { return new MemoryStream(img); });
            }
        }
    }
}