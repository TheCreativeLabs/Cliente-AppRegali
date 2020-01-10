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
using System.Collections.ObjectModel;

namespace AppRegali.Views
{
     //Learn more about making custom code visible in the Xamarin.Forms previewer
     //by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Impostazioni : ContentPage
    {

        public class MenuItem
        {
            public int Id { get; set; }
            public Page RedirectPage { get; set; }
            public string DisplayName { get; set; }
            public string Icona { get; set; }
        }

        public Impostazioni()
        {
            InitializeComponent();

            ObservableCollection<MenuItem> items = new ObservableCollection<MenuItem>();
            items.Add(new MenuItem() { Id = 0, DisplayName = "Informazioni personali", Icona = "\uf007", RedirectPage = new Account.Account(null) }); //fixme null
            items.Add(new MenuItem() { Id = 1, DisplayName = "Cambia password", Icona = "\uf084", RedirectPage = new Login.CambiaPassword() });
            items.Add(new MenuItem() { Id = 3, DisplayName = "Cambia lingua", Icona = "", RedirectPage = null });
            items.Add(new MenuItem() { Id = 4, DisplayName = "Condividi l'app", Icona = "\uf14d", RedirectPage = null });
            items.Add(new MenuItem() { Id = 6, DisplayName = "Informativa sulla privacy", Icona = "\uf505", RedirectPage = null }); //new GeneralCondition.PrivacyPolicy(
            items.Add(new MenuItem() { Id = 7, DisplayName = "Logout", Icona = "\uf2f5", RedirectPage = null });


            listView.ItemsSource = items;
        }

        async void OnItemTapped(object obj, ItemTappedEventArgs e)
        {
            var item = e.Item as MenuItem;

            if (item == null)
                return;

            listView.SelectedItem = null;

            if (item.Id == 7)
            {
                string action = await DisplayActionSheet("Procedere con il logout?", "Annulla", "Log out");
                if (action == "Log out")
                {
                    Logout();
                }
            }
            //else if (item.Id == 4)
            //{
            //    await ShareUri();
            //}
            //else if (item.Id == 3)
            //{
            //    await ContactUs();
            //}
            //else
            //{
            //    await Navigation.PushAsync(item.RedirectPage);
            //}
        }

        private async void Logout()
        {
            try
            {
                DependencyService.Get<IClearCookies>().ClearAllCookies();

                //Eseguo il logout
                AccountClient accountClient = new AccountClient(await ApiHelper.GetApiClient());
                await accountClient.LogoutAsync();

                //Rimuovo il token e navigo alla home
                Api.ApiHelper.RemoveSettings();
                Application.Current.MainPage = new Login.Login();
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }

    }
}