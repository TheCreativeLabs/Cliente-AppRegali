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

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventiPersonali : ContentPage
    {
        EventiViewModel viewModel;
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        //private EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());

        public string textModifica = Helpers.TranslateExtension.ResMgr.Value.GetString("EventiPersonali.Modifica", translate.ci);
        public string textEliminaEvento = Helpers.TranslateExtension.ResMgr.Value.GetString("EventiPersonali.EliminaEvento", translate.ci);

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
            await Navigation.PushModalAsync(new NavigationPage(new EventoInserisci()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //ricarico ogni volta per recepire le modifiche
            viewModel.LoadItemsCommand.Execute(null);
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