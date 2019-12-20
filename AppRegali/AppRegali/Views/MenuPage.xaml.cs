using Api;
using AppRegali.Api;
using AppRegali.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        UserInfoDto userInfoDto;

        public MenuPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Navigation.PushAsync(new ErrorPage());
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                await UpdateMenuData(MenuItemType.Home);
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }

        public async Task UpdateMenuData(MenuItemType currentPage)
        {
            //visualizzo le informazioni sull'utente.
            SetUserInfo();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Home, Title="Home", Icon="\uf015" },
                new HomeMenuItem {Id = MenuItemType.EventiPersonali, Title="I miei eventi", Icon="\uf073" },
                new HomeMenuItem {Id = MenuItemType.Amici, Title="I miei amici", Icon="\uf500"  },
                new HomeMenuItem {Id = MenuItemType.Account, Title="Account", Icon="\uf007", Model = userInfoDto  },
            };

            ListViewMenu.ItemsSource = menuItems;

            //ListViewMenu.SelectedItem = menuItems.Where(x => x.Id == currentPage).FirstOrDefault();
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                HomeMenuItem homeMenuItem = (HomeMenuItem)e.SelectedItem;

                await RootPage.NavigateFromMenu((int)homeMenuItem.Id, homeMenuItem.Model);
            };
        }

        /// <summary>
        /// Mostra nel menu le informazioni sull'utente
        /// </summary>
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
                    }else if (userInfoDto.PhotoUrl != null)
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
                await Navigation.PushAsync(new NavigationPage( new ErrorPage()));
            }
        }

        private async void btnLogout_Clicked(object sender, EventArgs e)
        {
            try
            {
                DependencyService.Get<IClearCookies>().ClearAllCookies();


                //if (Api.ApiHelper.GetFacebookLogin())
                //{
                //    //Vado alla pagina di logout di facebook
                //    Application.Current.MainPage = new NavigationPage(new Account.FacebookLogout());
                //}
                //else
                //{
                    //Eseguo il logout
                    AccountClient accountClient = new AccountClient(ApiHelper.GetApiClient());
                    await accountClient.LogoutAsync();

                    //Rimuovo il token e navigo alla home
                    Api.ApiHelper.RemoveSettings();
                    Application.Current.MainPage = new Login.Login();
                //}
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }
    }
}