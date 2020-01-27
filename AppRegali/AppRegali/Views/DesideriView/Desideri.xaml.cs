using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Api;
using AppRegali.ViewModels;
using AppRegali.Views.DesideriView;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Desideri : ContentPage
    {
        DesideriViewModel viewModel;

        public Desideri()
        {
            InitializeComponent();

            BindingContext = viewModel = new DesideriViewModel();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

                viewModel.LoadItemsCommand.Execute(null);
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DesideriInserisci());
        }

        async void MyListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item != null)
                await Navigation.PushAsync(new DesideriModifica((DesiderioDto)e.Item));
        }
    }
}
