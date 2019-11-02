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
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        public CategorieViewModel categorieViewModel { get; set; }

        public EventoModifica(EventoDetailViewModel eventoDetailViewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = eventoDetailViewModel;
            categorieViewModel = new CategorieViewModel();
        }

        protected override async void OnAppearing()
        {
            //Stream stream = new MemoryStream(viewModel.Item.ImmagineEvento);
            //imgEventoModifica.Source = ImageSource.FromStream(() => { return stream; });
            base.OnAppearing();

            if (categorieViewModel.Items.Count == 0)
            {
                categorieViewModel.LoadItemsCommand.Execute(null);
            }
            //pkCategoriaModifica.ItemsSource = categorieViewModel.Items;
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
            await DisplayAlert(null, 
                Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.SalvataggioOk", translate.ci),
                Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Ok", translate.ci));
            this.viewModel = new EventoDetailViewModel(eventoInserito);

            //Torno alla pagina di lista
            //await Navigation.PopModalAsync();
        }
    }
}