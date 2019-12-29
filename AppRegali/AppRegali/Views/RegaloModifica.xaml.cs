﻿using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [DesignTimeVisible(false)]
    public partial class RegaloModifica : ContentPage
    {
        RegaloDetailViewModel viewModel;
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        

        public RegaloModifica(RegaloDetailViewModel RegaloDetailViewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = RegaloDetailViewModel;
        }

        private async void Update_Clicked(object sender, EventArgs e)
        {
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

            Guid id = new Guid(viewModel.Item.Id);
            //Faccio update del regalo
            //var regaloAggiornato =
            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
            await eventoClient.UpdateRegaloAsync(id, regaloDtoInput);
            await DisplayAlert(null,
                Helpers.TranslateExtension.ResMgr.Value.GetString("RegaloModifica.SalvataggioOk", translate.ci),
                Helpers.TranslateExtension.ResMgr.Value.GetString("RegaloModifica.Ok", translate.ci));
            //TODO APPENA RIPUBBLICO API this.viewModel = new EventoDetailViewModel(eventoInserito);

        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await DisplayAlert("Attenzione", "Vuoi davvero eliminare il regalo?", "Yes", "No");
                if (answer)
                {
                    try
                    {
                        EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
                        await eventoClient.DeleteRegaloAsync(new Guid(viewModel.Item.Id));
                        await DisplayAlert(null, "Regalo eliminato", "Ok");
                        //torno indietro alla lista degli eventi personali
                        await Navigation.PopAsync();
                    }
                    catch
                    {
                        await DisplayAlert(null, "Errore durante l'eliminazione del regalo", "Ok");
                    }
                }

            }
            catch (Exception)
            {
                throw;
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

                viewModel.Item.ImmagineRegalo = memoryStream.ToArray();
                imgRegaloModifica.Source = ImageSource.FromStream(() => { return new MemoryStream(viewModel.Item.ImmagineRegalo); });
            }
        }

    }
}