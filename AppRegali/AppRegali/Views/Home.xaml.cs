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
using AppRegali.Api;
using AppRegali.Utility;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Home: ContentPage
    {
        EventiViewModel viewModel;
        EventoClient eventoClient = new EventoClient(ApiHelper.GetApiClient());
        ICollection<EventoCategoria> categorie;
        //static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();


        public Home()
        {

            InitializeComponent();

            BindingContext = viewModel = new EventiViewModel(false);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as EventoDtoOutput;
            if (item == null || item.Id == null)
                return;

            await Navigation.PushAsync(new EventoDettaglio(item));

            // Manually deselect item.

            EventiListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new EventoInserisci()));
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();

            if (categorie == null || categorie.Count == 0)
            {
                ICollection<EventoCategoria> categorie = eventoClient.GetLookupEventoCategoriaAsync().Result;
                List<EventoCategoria> listCategorie = categorie.ToList();

                listCategorie.Insert(0, new EventoCategoria() { Id = null, Codice = "ALL_CATEGORIES"});

                pkCategoria.ItemsSource = listCategorie;
            }

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            
        }

        private async void btnAbilitaRicerca_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RicercaAmici());
        }

        private void pkCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventoCategoria categoriaSelected = (EventoCategoria)pkCategoria.SelectedItem;
            entCategoria.Text = Helpers.TranslateExtension.ResMgr.Value.GetString((categoriaSelected).Codice, CurrentCulture.Ci);
            viewModel.Categoria = categoriaSelected;
            viewModel.LoadItemsCommand.Execute(null);
        }

        private async void btnCambiaLingua_Clicked(object sender, EventArgs e)
        {
            if(CurrentCulture.Ci.Name == "en")
            {
                CurrentCulture.Instance.SetCultureInfo("it");
            } else if (CurrentCulture.Ci.Name == "it")
            {
                CurrentCulture.Instance.SetCultureInfo("en");
            }

            MenuPage menuPage = (MenuPage)((MasterDetailPage)Application.Current.MainPage).Master;
            await menuPage.UpdateMenuData(MenuItemType.Home);
            Application.Current.MainPage = new MainPage();

        }


            private void entCategoria_Focused(object sender, FocusEventArgs e)
        {
            entCategoria.Unfocus();
            pkCategoria.Focus();
        }
    }
}