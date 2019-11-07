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
        EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());
        public CategorieViewModel categorieViewModel { get; set; }

        public EventoModifica(EventoDetailViewModel eventoDetailViewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = eventoDetailViewModel;
        }

        private async Task<int> LoadEventoDetailById(Guid id)
        {
            EventoDtoOutput eventoModel = await eventoClient.GetEventoByIdAsync(id);
            this.viewModel.Item = eventoModel;
            return 1;
        }

        protected override async void OnAppearing()
        {
            //Stream stream = new MemoryStream(viewModel.Item.ImmagineEvento);
            //imgEventoModifica.Source = ImageSource.FromStream(() => { return stream; });
            base.OnAppearing();

            await LoadEventoDetailById(new Guid(this.viewModel.Item.Id));
            
            List<EventoCategoria> listaCategorie = (List<EventoCategoria>)await this.eventoClient.GetLookupEventoCategoriaAsync();

            pkCategoria.ItemsSource = listaCategorie;
            EventoCategoria categoria = listaCategorie.First(a => a.Id == this.viewModel.Item.IdCategoriaEvento);
            pkCategoria.SelectedItem = categoria;
            //entCategoria.Text = categoria.Codice;
        }
        private void pkCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pkCategoria.SelectedItem != null)
            {
                entCategoria.Text = Helpers.TranslateExtension.ResMgr.Value.GetString(((EventoCategoria)pkCategoria.SelectedItem).Codice, translate.ci);
            }
        }

        private void entCategoria_Focused(object sender, FocusEventArgs e)
        {
            entCategoria.Unfocus();
            pkCategoria.Focus();
        }


        private async void Update_Clicked(object sender, EventArgs e)
        {
            

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
            //TODO APPENA RIPUBBLICO API this.viewModel = new EventoDetailViewModel(eventoInserito);

        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await DisplayAlert("Attenzione", "Vuoi davvero eliminare l'evento? Anche i regali di questo evento verranno rimossi.", "Yes", "No");
                if (answer)
                {
                    try
                    {
                        await eventoClient.DeleteEventoAsync(new Guid(viewModel.Item.Id));
                        await DisplayAlert(null, "Evento eliminato", "Ok");
                        //torno indietro alla lista degli eventi personali
                        await Navigation.PopAsync();
                    }
                    catch
                    {
                        await DisplayAlert(null, "Errore durante l'eliminazione dell'evento", "Ok");
                    }
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        async void AddRegalo_Clicked(object sender, EventArgs e)
        {
            RegaloDtoOutput regaloNew = new RegaloDtoOutput();
            regaloNew.IdEvento = new Guid(viewModel.Item.Id);
            await Navigation.PushModalAsync(new NavigationPage(new RegaloInserisci(new RegaloDetailViewModel(regaloNew))));
        }

        async void OnRegaloSelected(object sender, SelectedItemChangedEventArgs args)
        {
            RegaloDtoOutput item = args.SelectedItem as RegaloDtoOutput;
            await Navigation.PushAsync(new RegaloModifica(new RegaloDetailViewModel(item)));
           //await Navigation.PushModalAsync
        }
    }
}