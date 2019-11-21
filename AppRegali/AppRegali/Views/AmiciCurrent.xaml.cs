using Api;
using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AmiciCurrent : ContentPage
    {
        AmiciViewModel viewModel;

        public AmiciCurrent()
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

        async void OnContattiSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserInfoDto current = (e.CurrentSelection.FirstOrDefault() as UserInfoDto);

            if (current == null || current.IdAspNetUser == null)
                return;

            await Navigation.PushAsync(new AmiciProfilo(current.IdAspNetUser));
        }
    }
}