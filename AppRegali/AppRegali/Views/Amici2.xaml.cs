using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using System.ComponentModel;
using AppRegali.ViewModels;
using Api;

namespace AppRegali.Views
{
    [DesignTimeVisible(false)]
    public partial class Amici2 : ContentPage
    {
        AmiciViewModel viewModel;


        public Amici2()
        {
            InitializeComponent();
            BindingContext = viewModel = new AmiciViewModel(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //ricarico ogni volta per recepire le modifiche
            viewModel.LoadItemsCommand.Execute(null);
        }

        void ShowContacts(object sender, EventArgs e)
        {
            btnContact.BackgroundColor = (Color)App.Current.Resources["PrimaryColor"];
            btnContact.TextColor = Color.White;
            btnRequests.BackgroundColor = Color.White;//(Color)App.Current.Resources["LightColor"];
            btnRequests.TextColor = (Color)App.Current.Resources["PrimaryColor"];
            viewModel.SoloAccettati = true;
            viewModel.LoadItemsCommand.Execute(null);
            //stkContatti.IsVisible = true;
            //stkRequests.IsVisible = false;
        }

        void ShowRequests(object sender, EventArgs e)
        {
            btnContact.BackgroundColor = Color.White;
            btnContact.TextColor = (Color)App.Current.Resources["PrimaryColor"];
            btnRequests.BackgroundColor = (Color)App.Current.Resources["PrimaryColor"];
            btnRequests.TextColor = Color.White;
            viewModel.SoloAccettati = false;
            viewModel.LoadItemsCommand.Execute(null);
            //stkContatti.IsVisible = false;
            //stkRequests.IsVisible = true;
        }

        async void OnContattiSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserInfoDto current = (e.CurrentSelection.FirstOrDefault() as UserInfoDto);

            if (current == null || current.IdAspNetUser == null)
                return;

            await Navigation.PushModalAsync(new NavigationPage(new AmiciProfilo(current)));
        }

        async void OnRichiesteSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserInfoDto current = (e.CurrentSelection.FirstOrDefault() as UserInfoDto);

            if (current == null || current.IdAspNetUser == null)
                return;

            await Navigation.PushModalAsync(new AmiciProfilo(current));
        }
    }
}
