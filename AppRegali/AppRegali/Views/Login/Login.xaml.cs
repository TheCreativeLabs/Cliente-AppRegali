using AppRegali.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            //kkk();
        }

        //public async void kkk()
        //{

        //    string action = await DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", "Photo Roll", "Email");
        //    Debug.WriteLine("Action: " + action);
        //}

        private async void btnAccedi_ClickedAsync(object sender, EventArgs e)
        {
            ApiHelper apiHelper = new ApiHelper();
            await apiHelper.SetTokenAsync(entUsername.Text, entPassword.Text, new Uri("https://www.appregaliapitest.com/Token"));


            var i = apiHelper.GetToken();
            Application.Current.MainPage = new MainPage();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PasswordDimenticata());
        }

        private void TapGestureRecognizer_lblRegistrati(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Registrazione());
        }
    }
}