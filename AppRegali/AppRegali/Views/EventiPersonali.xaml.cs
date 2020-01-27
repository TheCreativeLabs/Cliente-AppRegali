using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppRegali.Models;
using AppRegali.Views;
using AppRegali.ViewModels;
using Api;
//using Java.Util;
using AppRegali.Api;
using AppRegali.Utility;
using System.Collections.ObjectModel;
using System.IO;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventiPersonali : ContentPage
    {
        EventiViewModel viewModel;
        UserInfoDto userInfoDto;
        List<UserInfoDto> amiciPreviewList = new List<UserInfoDto>();

        //static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        //private EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());

        public string textModifica = Helpers.TranslateExtension.ResMgr.Value.GetString("EventiPersonali.Modifica", CurrentCulture.Ci);
        public string textEliminaEvento = Helpers.TranslateExtension.ResMgr.Value.GetString("EventiPersonali.EliminaEvento", CurrentCulture.Ci);

        public EventiPersonali()
        {
            InitializeComponent();

            BindingContext = viewModel = new EventiViewModel(true);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as EventoDtoOutput;

            if (item == null || item.Id == null)
                return;

            await Navigation.PushAsync(new EventoModifica(new Guid(item.Id)));

            // Manually deselect item.
            EventiListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventoInserisci());
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<EventoModifica, string>(this, "RefreshListaEventiPersonaliModifica", async (sender, arg) =>
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    viewModel.Items = new ObservableCollection<EventoDtoOutput>();
                }
            });

            MessagingCenter.Subscribe<EventoInserisci, string>(this, "RefreshListaEventiPersonaliInsert", async (sender, arg) =>
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    viewModel.Items = new ObservableCollection<EventoDtoOutput>();
                }
            });

            MessagingCenter.Subscribe<EventoModifica, string>(this, "RefreshListaEventiPersonaliElimina", async (sender, arg) =>
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    viewModel.Items = new ObservableCollection<EventoDtoOutput>();
                }
            });


            //ricarico quando devo recepire le modifiche
            if (viewModel.Items == null || viewModel.Items.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }

            if(amiciPreviewList == null || !amiciPreviewList.Any())
            {
                AmiciClient amiciClient = new AmiciClient(await ApiHelper.GetApiClient());
                ICollection<UserInfoDto> collect = await amiciClient.GetAmiciCurrentUserPreviewAsync();
                amiciPreviewList = collect.ToList();
                AmiciPreviewListView.ItemsSource = amiciPreviewList;
            }


            await SetUserInfo();
        }

        private async Task SetUserInfo()
        {
            try
            {
                UserInfoDto userInfo = await ApiHelper.GetUserInfo();

                if (userInfo != null)
                {
                    userInfoDto = userInfo;

                    if (userInfoDto.FotoProfilo != null)
                    {
                        Stream stream = new MemoryStream(userInfo.FotoProfilo);
                        imgFotoUtente.Source = ImageSource.FromStream(() => { return stream; });
                    }
                    else if (userInfoDto.PhotoUrl != null)
                    {
                        imgFotoUtente.Source = ImageSource.FromUri(new Uri(userInfoDto.PhotoUrl));
                    }

                    lblNomeCognome.Text = $"{userInfo.Nome} {userInfo.Cognome}";
                    lblEmail.Text = userInfo.Email;
                }
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new NavigationPage(new ErrorPage()));
            }
        }

        
        async void OnAmiciViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as UserInfoDto;

            if (item == null)
                return;

            //await Navigation.PushModalAsync(new NavigationPage(new RegaloPersonaleDettaglio(item)));
            await Navigation.PushAsync(new AmiciProfilo(item));

        }

        private async void TapAllFriends_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Amici2());

            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();
            }
        }

        private async void btn_ModificaAccountClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Account.Account(userInfoDto));
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();
            }
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Account.Account(userInfoDto));
        }
        //private async void lblComandiRapidi_Tapped(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        string action = await DisplayActionSheet(Helpers.TranslateExtension.ResMgr.Value.GetString("EventiPersonali.ComandiRapidi", translate.ci),
        //                                                 Helpers.TranslateExtension.ResMgr.Value.GetString("EventiPersonali.Annulla", translate.ci), null,
        //                                                 textModifica,
        //                                                 textEliminaEvento);
        //        if(action == textModifica)
        //        {

        //        }
        //        else if(action == textEliminaEvento)
        //        {
        //            bool answer = await DisplayAlert("Attenzione", "Vuoi davvero eliminare l'evento? Anche i regali di questo evento verranno rimossi.", "Yes", "No");
        //            if (answer)
        //            {
        //                var idEvento = ((Button)sender).CommandParameter;
        //                try
        //                {
        //                    await eventoClient.DeleteEventoAsync(new Guid((string)idEvento));
        //                    await DisplayAlert(null, "Evento eliminato", "Ok");
        //                    //ricarico la lista degli eventi in modo che non si visualizzi più l'evento eliminato
        //                    viewModel.LoadItemsCommand.Execute(null);
        //                }
        //                catch
        //                {
        //                    await DisplayAlert(null, "Errore durante l'eliminazione dell'evento", "Ok");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}