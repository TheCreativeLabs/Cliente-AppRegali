using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
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
            EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());

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
            await eventoClient.InserisciRegaloAsync(regaloDtoInput);
            await Navigation.PopModalAsync();
            //await DisplayAlert(null,
            //    Helpers.TranslateExtension.ResMgr.Value.GetString("RegaloModifica.SalvataggioOk", translate.ci),
            //    Helpers.TranslateExtension.ResMgr.Value.GetString("RegaloModifica.Ok", translate.ci));
            //TODO POPModelAsync

        }
    }
}