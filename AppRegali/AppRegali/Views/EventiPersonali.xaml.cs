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
using Java.Util;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventiPersonali : ContentPage
    {
        EventiViewModel viewModel;
        Helpers.TranslateExtension translate = new Helpers.TranslateExtension();

        public EventiPersonali()
        {
            InitializeComponent();

            BindingContext = viewModel = new EventiViewModel(true);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as EventoDtoOutput;
            if (item == null)
                return;

            await Navigation.PushAsync(new EventoDettaglio(new EventoDetailViewModel(item)));

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

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        private async void lblComandiRapidi_Tapped(object sender, EventArgs e)
        {
            try
            {

                Helpers.TranslateExtension i = new Helpers.TranslateExtension();

                i.Text = "Account.CambiaPassword";
               var o = Helpers.TranslateExtension.ResMgr.Value.GetString(, translate.ci);

                string action = await DisplayActionSheet("Comandi rapidi", "Annulla", "Elimina evento", "Modifica");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}