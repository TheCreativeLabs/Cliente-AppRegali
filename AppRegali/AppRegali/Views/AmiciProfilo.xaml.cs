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
    public partial class AmiciProfilo : ContentPage
    {

        UserProfiloViewModel viewModel;

        public AmiciProfilo(UserInfoDto userInfo)
        {
            InitializeComponent();

            BindingContext = viewModel = new UserProfiloViewModel(userInfo);
            viewModel.LoadItemsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //ricarico ogni volta per recepire le modifiche
            //viewModel.LoadItemsCommand.Execute(null);
        }
    }
}