using System;
using System.Collections.Generic;
using System.Linq;
using Api;
using AppRegali.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.DesideriView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegaloDaDesiderio : ContentPage
    {
        DesideriViewModel viewModel;

        public RegaloDaDesiderio()
        {
            InitializeComponent();

            BindingContext = viewModel = new DesideriViewModel();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!viewModel.Items.Any())
                viewModel.LoadItemsCommand.Execute(null);
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DesideriInserisci());
        }

        async void MyListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var obj = e.Item as DesiderioDto;
                MessagingCenter.Send(this, "RegaloDaDesiderio", JsonConvert.SerializeObject(obj));
                 await Navigation.PopAsync();

            }

        }
    }
}
