using Api;
using AppRegali.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

            //visualizzo le informazioni sull'utente.
            SetUserInfo();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Home, Title="Home", Icon="\uf015" },
                new HomeMenuItem {Id = MenuItemType.EventiPersonali, Title="I miei eventi", Icon="\uf271" },
                new HomeMenuItem {Id = MenuItemType.Amici, Title="I miei amici", Icon="\uf0c0"  },
                new HomeMenuItem {Id = MenuItemType.Account, Title="Account", Icon="\uf2bd"  },
            };

            ListViewMenu.ItemsSource = menuItems;
            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;

                await RootPage.NavigateFromMenu(id);
            };
        }


        /// <summary>
        /// Mostra nel menu le informazioni sull'utente
        /// </summary>
        private async void SetUserInfo()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());

                AmiciClient amiciClient = new AmiciClient(httpClient);

                UserInfo userInfo = await amiciClient.GetCurrentUserInfoAsync();

                if (userInfo != null)
                {
                    Image image = new Image();
                    Stream stream = new MemoryStream(userInfo.FotoProfilo);
                    imgFotoUtente.Source = ImageSource.FromStream(() => { return stream; });
                    lblNomeCognome.Text = $"{userInfo.Nome} {userInfo.Cognome}";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Rimuovo il token e navigo alla home
                Api.ApiHelper.DeleteToken();
                Application.Current.MainPage = new NavigationPage(new Login.Login());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}