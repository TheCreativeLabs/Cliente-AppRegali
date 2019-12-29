using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
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
    public partial class EventoModifica : ContentPage
    {
        EventoDetailViewModel viewModel;
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        public CategorieViewModel categorieViewModel { get; set; }
        Guid EventoId;

        EventoClient eventoClient;

        public EventoModifica(Guid Id)
        {
            InitializeComponent();
            EventoId = Id;

        }

        private async Task<EventoDtoOutput> LoadEventoDetailById()
        {
            EventoDtoOutput eventoModel = await eventoClient.GetEventoByIdAsync(EventoId);
            return eventoModel;
        }

        protected override async void OnAppearing()
        {
            //Stream stream = new MemoryStream(viewModel.Item.ImmagineEvento);
            //imgEventoModifica.Source = ImageSource.FromStream(() => { return stream; });
            base.OnAppearing();

            eventoClient = new EventoClient(await ApiHelper.GetApiClient());

            EventoModificaActivityIndicator.IsRunning = true;
            EventoModificaActivityIndicator.IsVisible = true;

            //if (this.viewModel == null) ricarico SEMPRE e non solo se è null, altrimenti non si percepiscono le modifiche dei regali
            //{
                this.viewModel = new EventoDetailViewModel();
                this.viewModel.Item = await LoadEventoDetailById();
                BindingContext = this.viewModel;

                RegaliModificaListView.ItemsSource = viewModel.Item.Regali;

               
                List<EventoCategoria> listaCategorie = (List<EventoCategoria>)await this.eventoClient.GetLookupEventoCategoriaAsync();
                pkCategoria.ItemsSource = listaCategorie;
                EventoCategoria categoria = listaCategorie.First(a => a.Id == this.viewModel.Item.IdCategoriaEvento);
                pkCategoria.SelectedItem = categoria;
            //}

            EventoModificaActivityIndicator.IsRunning = false;
            EventoModificaActivityIndicator.IsVisible = false;
            ScrollViewEventoModifica.IsVisible = true;
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
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await DisplayAlert("Attenzione",
                    Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.ConfirmDelete", translate.ci),
                    Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Yes", translate.ci),
                    Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.No", translate.ci));
                
                if (answer)
                {
                    try
                    {
                        await eventoClient.DeleteEventoAsync(new Guid(viewModel.Item.Id));
                        await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.EventDeleted", translate.ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Ok", translate.ci));
                        //torno indietro alla lista degli eventi personali
                        await Navigation.PopAsync();
                    }
                    catch
                    {
                        await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.DeleteError", translate.ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Ok", translate.ci)); //FIXME
                    }
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void BtnModificaRegalo_Clicked(object sender, EventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                var item = (RegaloDtoOutput)btn.CommandParameter;
                await Navigation.PushAsync(new RegaloModifica(new RegaloDetailViewModel(item)));
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
                viewModel.Item.DataEvento = dpDataEvento.Date;
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

                viewModel.Item.ImmagineEvento = memoryStream.ToArray();
                imgEventoModifica.Source = ImageSource.FromStream(() => { return new MemoryStream(viewModel.Item.ImmagineEvento); });
            }
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as RegaloDtoOutput;

            if (item == null || item.Id == null)
                return;

            await Navigation.PushModalAsync(new RegaloPersonaleDettaglio(item));

            RegaliModificaListView.SelectedItem = null;
        }
    }
}