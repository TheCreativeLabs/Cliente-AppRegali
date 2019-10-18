using Api;
using AppRegali.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());
            AccountClient accountClient = new AccountClient(httpClient);

            //UserInfo userInfo = accountClient.GetUserDetailAsync().Result;

            //if (userInfo != null)
            //{
            //    lblNomeCognome.Text = $"{userInfo.Nome} {userInfo.Cognome}";
            //    lblEmail.Text = "";
            //}

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse" },
                new HomeMenuItem {Id = MenuItemType.Account, Title="Account" },
                new HomeMenuItem {Id = MenuItemType.LogOut, Title="LogOut" }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;

                //Se l'item selezionato è "LogOut" allora torno alla pagina di Login
                if (id == 2)
                {
                    Application.Current.MainPage = new NavigationPage(new Login.Login());
                }
                else
                {
                    await RootPage.NavigateFromMenu(id);
                }

            };
        }
    }
}