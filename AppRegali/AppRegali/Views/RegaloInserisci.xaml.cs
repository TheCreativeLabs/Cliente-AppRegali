using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
using AppRegali.Views.DesideriView;
using DependencyServiceDemos;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegaloInserisci : ContentPage
    {
        RegaloDetailViewModel viewModel;
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();

        public RegaloInserisci(RegaloDetailViewModel RegaloDetailViewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = RegaloDetailViewModel;

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<RegaloDaDesiderio, string>(this, "RegaloDaDesiderio", async (sender, arg) =>
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    DesiderioDto regaloDtoOutput = JsonConvert.DeserializeObject<DesiderioDto>(arg);
                    entTitoloRegalo.Text = regaloDtoOutput.Titolo;
                    entPrezzoRegalo.Text = regaloDtoOutput.Prezzo.ToString();
                    entDescrizioneRegalo.Text = regaloDtoOutput.Descrizione;
                    viewModel.Item.ImmagineRegalo = regaloDtoOutput.ImmagineDesiderio;
                    imgRegaloModifica.Source = ImageSource.FromStream(() => { return new MemoryStream(regaloDtoOutput.ImmagineDesiderio); });
                }
            });
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {

            RegaloInserisciActivityIndicator.IsVisible = true;

            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());

            RegaloDtoInput regaloDtoInput = new RegaloDtoInput()
            {
                Cancellato = viewModel.Item.Cancellato,
                Descrizione = viewModel.Item.Descrizione,
                IdEvento = viewModel.Item.IdEvento,
                ImmagineRegalo = viewModel.Item.ImmagineRegalo,
                ImportoCollezionato = viewModel.Item.ImportoCollezionato,
                Prezzo = viewModel.Item.Prezzo,
                Titolo = viewModel.Item.Titolo
            };

            //Creo il regalo
            RegaloDtoOutput regaloInserito = await eventoClient.InserisciRegaloAsync(regaloDtoInput);

            MessagingCenter.Send(this, "RefreshListaRegaliPersonaliInserisci", "OK");

            RegaloInserisciActivityIndicator.IsVisible = false;

            await Navigation.PopModalAsync();

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

                viewModel.Item.ImmagineRegalo = memoryStream.ToArray();
                imgRegaloModifica.Source = ImageSource.FromStream(() => { return new MemoryStream(viewModel.Item.ImmagineRegalo); });
            }
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            RegaloInserisciActivityIndicator.IsVisible = true;

            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());

            RegaloDtoInput regaloDtoInput = new RegaloDtoInput()
            {
                Cancellato = viewModel.Item.Cancellato,
                Descrizione = viewModel.Item.Descrizione,
                IdEvento = viewModel.Item.IdEvento,
                ImmagineRegalo = viewModel.Item.ImmagineRegalo,
                ImportoCollezionato = viewModel.Item.ImportoCollezionato,
                Prezzo = viewModel.Item.Prezzo,
                Titolo = viewModel.Item.Titolo
            };

            //Creo il regalo
            RegaloDtoOutput regaloInserito = await eventoClient.InserisciRegaloAsync(regaloDtoInput);

            MessagingCenter.Send(this, "RefreshListaRegaliPersonaliInserisci", "OK");

            RegaloInserisciActivityIndicator.IsVisible = false;

            await Navigation.PopModalAsync();

        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            if(!string.IsNullOrEmpty(entTitoloRegalo.Text) &&
                !string.IsNullOrEmpty(entDescrizioneRegalo.Text) &&
                !string.IsNullOrEmpty(entPrezzoRegalo.Text))
            {


            RegaloInserisciActivityIndicator.IsVisible = true;

            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());

            RegaloDtoInput regaloDtoInput = new RegaloDtoInput()
            {
                Cancellato = viewModel.Item.Cancellato,
                Descrizione = entDescrizioneRegalo.Text,
                IdEvento = viewModel.Item.IdEvento,
                ImmagineRegalo = viewModel.Item.ImmagineRegalo,
                ImportoCollezionato = viewModel.Item.ImportoCollezionato,
                Prezzo = int.Parse(entPrezzoRegalo.Text),
                Titolo = entTitoloRegalo.Text
            };

            //Creo il regalo
            RegaloDtoOutput regaloInserito = await eventoClient.InserisciRegaloAsync(regaloDtoInput);

            MessagingCenter.Send(this, "RefreshListaRegaliPersonaliInserisci", "OK");

            RegaloInserisciActivityIndicator.IsVisible = false;

            await Navigation.PopAsync();

            }
            else
            {
                await DisplayAlert("Attenzione", "è necessario compilare tutti i campi.", "OK");
            }
        }

        async void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RegaloDaDesiderio());
        }
    }
}