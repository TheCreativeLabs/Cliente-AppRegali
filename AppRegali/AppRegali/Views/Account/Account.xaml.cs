using Api;
using AppRegali.ViewModels;
using DependencyServiceDemos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Account
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Account : ContentPage
    {
        UserInfoDto viewModel;

        public Account()
        {
            InitializeComponent();

        }

        private void btnCambiaPassword_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new Login.CambiaPassword());
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel == null)
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());
                AmiciClient amiciClient = new AmiciClient(httpClient);
                UserInfoDto userInfo = await amiciClient.GetCurrentUserInfoAsync();

                viewModel = userInfo;
                BindingContext = viewModel;
            }

            Image image = new Image();
            Stream stream = new MemoryStream(viewModel.FotoProfilo);
            imgFotoUtente.Source = ImageSource.FromStream(() => { return stream; });
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                imgFotoUtente.Source = ImageSource.FromStream(() => stream);
            }

            (sender as Button).IsEnabled = true;
        }

        private void ent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Controllo che username e password siano valorizzati.
                //if (!(String.IsNullOrEmpty(entNome.Text)) && !(String.IsNullOrEmpty(entCognome.Text))
                //    && !(String.IsNullOrEmpty(entPassword.Text)) && !(String.IsNullOrEmpty(entConfermaPassword.Text)) && !(String.IsNullOrEmpty(entEmail.Text)))
                //{
                //    btnRegistrati.IsEnabled = true;
                //}
                //else
                //{
                //    btnRegistrati.IsEnabled = false;
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}