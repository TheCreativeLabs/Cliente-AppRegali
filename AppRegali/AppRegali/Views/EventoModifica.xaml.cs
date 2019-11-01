using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
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
    public partial class EventoModifica : ContentPage
    {
        EventoDetailViewModel viewModel;

        public EventoModifica(EventoDetailViewModel eventoDetailViewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = eventoDetailViewModel;
        }

        protected override async void OnAppearing()
        {
            //Stream stream = new MemoryStream(viewModel.Item.ImmagineEvento);
            //imgEventoModifica.Source = ImageSource.FromStream(() => { return stream; });
        }

        private async void Update_Clicked(object sender, EventArgs e)
        {
            EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());

            EventoDtoInput eventoDtoInput = new EventoDtoInput()
            {
                Cancellato = viewModel.Item.Cancellato,
                DataEvento = viewModel.Item.DataEvento,
                Descrizione = viewModel.Item.Descrizione,
                IdCategoriaEvento = viewModel.Item.IdCategoriaEvento,
                ImmagineEvento = viewModel.Item.ImmagineEvento,
                Titolo = viewModel.Item.Titolo
            };

            Guid id = new Guid(viewModel.Item.Id);
            //Faccio update dell'evento
            var eventoInserito = await eventoClient.UpdateEventoAsync(new Guid(viewModel.Item.Id), eventoDtoInput);
            await DisplayAlert(null, "Salvataggio eseguito con successo", "OK");

            //Torno alla pagina di lista
            //await Navigation.PopModalAsync();
        }
    }
}