using System;
using System.Collections.Generic;
using System.IO;
using Api;
using AppRegali.Api;
using AppRegali.Utility;
using AppRegali.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace AppRegali.Views.DesideriView
{
    public partial class DesideriModifica : ContentPage
    {
        DesiderioDto viewModel;

        public DesideriModifica(DesiderioDto Desiderio)
        {
            InitializeComponent();

           BindingContext= viewModel = Desiderio;
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

                viewModel.ImmagineDesiderio= memoryStream.ToArray();
                imgRegaloModifica.Source = ImageSource.FromStream(() => { return new MemoryStream(viewModel.ImmagineDesiderio); });
            }
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            DesideriClient desiderioClient = new DesideriClient(await ApiHelper.GetApiClient());

            await desiderioClient.UpdateRegaloAsync(viewModel.Id.Value,viewModel);
            await Navigation.PopAsync();
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            bool answer = await DisplayAlert("Attenzione",
                  Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.ConfirmDelete", CurrentCulture.Ci),
                  Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Yes", CurrentCulture.Ci),
                  Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.No", CurrentCulture.Ci));

            if (answer)
            {
                DesideriClient desiderioClient = new DesideriClient(await ApiHelper.GetApiClient());
                await desiderioClient.DeleteDesiderioAsync(viewModel.Id.Value);
                await Navigation.PopAsync();
            }
        }
    }
}
