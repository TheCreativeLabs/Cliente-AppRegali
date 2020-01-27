using System;
using System.Collections.Generic;
using System.IO;
using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace AppRegali.Views.DesideriView
{
    public partial class DesideriInserisci : ContentPage
    {
        DesiderioDto desiderioDto;

        public DesideriInserisci()
        {
            InitializeComponent();

            desiderioDto = new DesiderioDto();
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

                desiderioDto.ImmagineDesiderio= memoryStream.ToArray();
                imgRegaloModifica.Source = ImageSource.FromStream(() => { return new MemoryStream(desiderioDto.ImmagineDesiderio); });
            }
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            DesideriClient desiderioClient = new DesideriClient(await ApiHelper.GetApiClient());
            desiderioDto.Descrizione = entDescrizioneRegalo.Text;
            desiderioDto.Prezzo = int.Parse(entPrezzoRegalo.Text);
            desiderioDto.Titolo = entTitoloRegalo.Text;

            await desiderioClient.InserisciDesiderioAsync(desiderioDto);
            await Navigation.PopAsync();
        }
    }
}
