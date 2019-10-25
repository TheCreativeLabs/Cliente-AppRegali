using Api;
using AppRegali.Api;
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
            try
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
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Navigation.PushAsync(new ErrorPage());
            }
        }


        /// <summary>
        /// Mostra nel menu le informazioni sull'utente
        /// </summary>
        private async void SetUserInfo()
        {
            try
            {
                AmiciClient amiciClient = new AmiciClient(ApiHelper.GetApiClient());
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
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new NavigationPage( new ErrorPage()));
            }
        }

        private async void btnLogout_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Api.ApiHelper.GetFacebookLogin())
                {
                    //Vado alla pagina di logout di facebook
                    Application.Current.MainPage = new NavigationPage(new Account.FacebookLogout());
                }
                else
                {
                    //Eseguo il logout

                    AccountClient accountClient = new AccountClient(ApiHelper.GetApiClient());
                    await accountClient.LogoutAsync();

                    //Rimuovo il token e navigo alla home
                    Api.ApiHelper.DeleteToken();
                }
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }
    }
}