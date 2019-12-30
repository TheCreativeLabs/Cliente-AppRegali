using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
using DependencyServiceDemos;
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

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {
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
    }
}