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
        ObservableCollection<MenuItem> items;

        public class MenuItem
        {
            public int Id { get; set; }
            public Page RedirectPage { get; set; }
            public string DisplayName { get; set; }
            public string Icona { get; set; }
            public string Image { get; set; }
        }

        List<Language> languages;

        private class Language
        {
            public string Codice { get; set; }
            public string Local { get; set; }
        }

        public Impostazioni()
        {
            InitializeComponent();

            items = new ObservableCollection<MenuItem>();
            //items.Add(new MenuItem() { Id = 0, DisplayName = "Informazioni personali", Icona = "\uf007", RedirectPage = new Account.Account(null) }); //fixme null
            //items.Add(new MenuItem() { Id = 1, DisplayName = "Cambia password", Icona = "\uf084", RedirectPage = new Login.CambiaPassword() });
            items.Add(new MenuItem() { Id = 2, DisplayName = "Condividi l'app", Icona = "\uf14d", RedirectPage = null });
            items.Add(new MenuItem() { Id = 3, DisplayName = "Informativa sulla privacy", Icona = "\uf505", RedirectPage = null }); //new GeneralCondition.PrivacyPolicy(
            items.Add(new MenuItem() { Id = 4, DisplayName = "Cambia lingua", Icona = "", RedirectPage = null, Image="it.png" });
            items.Add(new MenuItem() { Id = 5, DisplayName = "Logout", Icona = "\uf2f5", RedirectPage = null });

            languages = new List<Language>()
                {
                    new Language(){ Codice="Language.it", Local="it" },
                    new Language(){ Codice="Language.en", Local="en" }
                };

            pkLanguage.ItemsSource = languages;

            listViewImpostazioni.ItemsSource = items;
        }

        async void OnItemTapped(object obj, ItemTappedEventArgs e)
        {
            var item = e.Item as MenuItem;

            if (item == null)
                return;

            listViewImpostazioni.SelectedItem = null;

            if (item.Id == 4)
            {
                pkLanguage.Focus();
                listViewImpostazioni.SelectedItem = null;
            }
            else if (item.Id == 5)
            {
                string action = await DisplayActionSheet("Procedere con il logout?", "Annulla", "Log out");
                if (action == "Log out")
                {
                    Logout();
                }
            }
            else if (item.Id == 2 || item.Id==3)
            {
            }
            else
            {
                await Navigation.PushAsync(item.RedirectPage);
            }
        }

        private async void btnCambiaLingua_Clicked(object sender, EventArgs e)
        {
            pkLanguage.Focus();

            if (CurrentCulture.Ci.Name == "en")
            {
                CurrentCulture.Instance.SetCultureInfo("it");
            }
            else if (CurrentCulture.Ci.Name == "it")
            {
                CurrentCulture.Instance.SetCultureInfo("en");
            }



            changeLanguageIcon();

            MenuPage menuPage = (MenuPage)((MasterDetailPage)Application.Current.MainPage).Master;
            await menuPage.UpdateMenuData(MenuItemType.Home);
            Application.Current.MainPage = new MainPage();
        }

        private async void pkLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language languageSelected = (Language)pkLanguage.SelectedItem;
            //entCategoria.Text = Helpers.TranslateExtension.ResMgr.Value.GetString((categoriaSelected).Codice, CurrentCulture.Ci);
            //viewModel.Categoria = categoriaSelected;
            //viewModel.LoadItemsCommand.Execute(null);

            CurrentCulture.Instance.SetCultureInfo(languageSelected.Local);

            changeLanguageIcon();


            //MenuPage menuPage = (MenuPage)((MasterDetailPage)Application.Current.MainPage).Master;
            //await menuPage.UpdateMenuData(MenuItemType.Home);
            //Application.Current.MainPage = new MainPage();
        }


        private void changeLanguageIcon()
        {
            //if (CurrentCulture.Ci.Name == "en")
            //{
            //    imgEn.IsVisible = true;
            //}
            //else if (CurrentCulture.Ci.Name == "it")
            //{
            //    imgIt.IsVisible = true;
            //}

            var itemLang = items.Where(x => x.Id == 4).FirstOrDefault();
            var item = itemLang;
            items.RemoveAt(2);
            item.Image = CurrentCulture.Ci.Name + ".png";
            items.Insert(2,item);
            listViewImpostazioni.ItemsSource = items;
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


        //List<HomeMenuItem> menuItems;

        //UserInfoDto userInfoDto;

        //List<Language> languages;

        //public Impostazioni()
        //{
        //    try
        //    {
        //        InitializeComponent();
        //    }
        //    catch (Exception ex)
        //    {
        //        //Navigo alla pagina d'errore.
        //        Navigation.PushAsync(new ErrorPage());
        //    }
        //}

        //private class Language
        //{
        //    public string Codice { get; set; }
        //    public string Local { get; set; }
        //}

        //protected override async void OnAppearing()
        //{
        //    try
        //    {
        //        base.OnAppearing();
        //        //changeLanguageIcon();

        //        languages = new List<Language>()
        //        {
        //            new Language(){ Codice="Language.it", Local="it" },
        //            new Language(){ Codice="Language.en", Local="en" }
        //        };

        //        pkLanguage.ItemsSource = languages;

        //        //SetUserInfo();

        //        await UpdateMenuData(MenuItemType.Home);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Navigo alla pagina d'errore.
        //        await Navigation.PushAsync(new ErrorPage());
        //    }
        //}

        //public async Task UpdateMenuData(MenuItemType currentPage)
        //{
        //    //visualizzo le informazioni sull'utente.
        //    //SetUserInfo();

        //    string homeTitle = Helpers.TranslateExtension.ResMgr.Value.GetString("Menu.Home", CurrentCulture.Ci);
        //    string mieiEventiTitle = Helpers.TranslateExtension.ResMgr.Value.GetString("Menu.MyEvents", CurrentCulture.Ci);
        //    string mieiAmiciTitle = Helpers.TranslateExtension.ResMgr.Value.GetString("Menu.MyFriends", CurrentCulture.Ci);
        //    string accountTitle = Helpers.TranslateExtension.ResMgr.Value.GetString("Menu.Account", CurrentCulture.Ci);
        //    string logOutTitle = Helpers.TranslateExtension.ResMgr.Value.GetString("Menu.Logout", CurrentCulture.Ci);
        //    string languageTitle = Helpers.TranslateExtension.ResMgr.Value.GetString("Menu.ChangeLanguage", CurrentCulture.Ci);

        //    menuItems = new List<HomeMenuItem>
        //    {


        //    //new HomeMenuItem {Id = MenuItemType.Home, Title=homeTitle, Icon="\uf015" },
        //    //    new HomeMenuItem {Id = MenuItemType.EventiPersonali, Title=mieiEventiTitle, Icon="\uf073" },
        //    //    new HomeMenuItem {Id = MenuItemType.Amici, Title=mieiAmiciTitle, Icon="\uf500"  },
        //    //    new HomeMenuItem {Id = MenuItemType.Account, Title=accountTitle, Icon="\uf007", Model = userInfoDto  },
        //        new HomeMenuItem {Id = MenuItemType.Logout, Title=logOutTitle, Icon="\uf2f5", Model = userInfoDto  },
        //        new HomeMenuItem {Id = MenuItemType.Language, Title=languageTitle }
        //    };

        //    changeLanguageIcon();

        //    ListViewMenu.ItemsSource = menuItems;

        //    //ListViewMenu.SelectedItem = menuItems.Where(x => x.Id == currentPage).FirstOrDefault();
        //    ListViewMenu.ItemSelected += async (sender, e) =>
        //    {
        //        if (e.SelectedItem == null)
        //            return;

        //        HomeMenuItem homeMenuItem = (HomeMenuItem)e.SelectedItem;

        //        if (homeMenuItem.Id == MenuItemType.Logout)
        //        {
        //            string action = await DisplayActionSheet("Procedere con il logout?", "Annulla", "Log out");
        //            if (action == "Log out")
        //            {
        //                Logout();
        //            }
        //        }
        //        else if (homeMenuItem.Id == MenuItemType.Language)
        //        {
        //            pkLanguage.Focus();
        //            ListViewMenu.SelectedItem = null;
        //        }
        //        else
        //        {
        //            //await RootPage.NavigateFromMenu((int)homeMenuItem.Id, homeMenuItem.Model);
        //        }
        //    };
        //}

        ///// <summary>
        ///// Mostra nel menu le informazioni sull'utente
        ///// </summary>
        ////private async Task SetUserInfo()
        ////{
        ////    try
        ////    {
        ////        UserInfoDto userInfo = await ApiHelper.GetUserInfo();

        ////        if (userInfo != null)
        ////        {
        ////            userInfoDto = userInfo;

        ////            if (userInfoDto.FotoProfilo != null)
        ////            {
        ////                Stream stream = new MemoryStream(userInfo.FotoProfilo);
        ////                imgFotoUtente.Source = ImageSource.FromStream(() => { return stream; });
        ////            }
        ////            else if (userInfoDto.PhotoUrl != null)
        ////            {
        ////                imgFotoUtente.Source = ImageSource.FromUri(new Uri(userInfoDto.PhotoUrl));
        ////            }

        ////            lblNomeCognome.Text = $"{userInfo.Nome} {userInfo.Cognome}";
        ////            lblEmail.Text = userInfo.Email;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        //Navigo alla pagina d'errore.
        ////        await Navigation.PushAsync(new NavigationPage(new ErrorPage()));
        ////    }
        ////}

        //private async void btnCambiaLingua_Clicked(object sender, EventArgs e)
        //{
        //    pkLanguage.Focus();

        //    //if (CurrentCulture.Ci.Name == "en")
        //    //{
        //    //    CurrentCulture.Instance.SetCultureInfo("it");
        //    //}
        //    //else if (CurrentCulture.Ci.Name == "it")
        //    //{
        //    //    CurrentCulture.Instance.SetCultureInfo("en");
        //    //}

        //    //changeLanguageIcon();

        //    //MenuPage menuPage = (MenuPage)((MasterDetailPage)Application.Current.MainPage).Master;
        //    //await menuPage.UpdateMenuData(MenuItemType.Home);
        //    //Application.Current.MainPage = new MainPage();

        //}

        //private async void pkLanguage_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    Language languageSelected = (Language)pkLanguage.SelectedItem;
        //    //entCategoria.Text = Helpers.TranslateExtension.ResMgr.Value.GetString((categoriaSelected).Codice, CurrentCulture.Ci);
        //    //viewModel.Categoria = categoriaSelected;
        //    //viewModel.LoadItemsCommand.Execute(null);

        //    CurrentCulture.Instance.SetCultureInfo(languageSelected.Local);

        //    changeLanguageIcon();


        //    MenuPage menuPage = (MenuPage)((MasterDetailPage)Application.Current.MainPage).Master;
        //    await menuPage.UpdateMenuData(MenuItemType.Home);
        //    Application.Current.MainPage = new MainPage();
        //}


        //private void changeLanguageIcon()
        //{
        //    //if (CurrentCulture.Ci.Name == "en")
        //    //{
        //    //    imgEn.IsVisible = true;
        //    //}
        //    //else if (CurrentCulture.Ci.Name == "it")
        //    //{
        //    //    imgIt.IsVisible = true;
        //    //}

        //    HomeMenuItem itemLang = menuItems.Where(x => x.Id == MenuItemType.Language).FirstOrDefault();
        //    itemLang.Image = CurrentCulture.Ci.Name + ".png";
        //    ListViewMenu.ItemsSource = menuItems.ToArray();
        //}

        //private async void Logout()
        //{
        //    try
        //    {
        //        DependencyService.Get<IClearCookies>().ClearAllCookies();

        //        //Eseguo il logout
        //        AccountClient accountClient = new AccountClient(await ApiHelper.GetApiClient());
        //        await accountClient.LogoutAsync();

        //        //Rimuovo il token e navigo alla home
        //        Api.ApiHelper.RemoveSettings();
        //        Application.Current.MainPage = new Login.Login();
        //    }
        //    catch (Exception ex)
        //    {
        //        //Navigo alla pagina d'errore.
        //        await Navigation.PushAsync(new ErrorPage());
        //    }
        //}

    }
}