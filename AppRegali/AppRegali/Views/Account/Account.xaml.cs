using Api;
using AppRegali.ViewModels;
using DependencyServiceDemos;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
        byte[] img;

        public Account( UserInfoDto Model)
        {
            InitializeComponent();

            viewModel = Model;
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                BindingContext = viewModel;

                if (img == null && viewModel.FotoProfilo != null)
                {
                    MemoryStream stream = new MemoryStream(viewModel.FotoProfilo);

                    if (stream != null)
                    {
                        imgFotoUtente.Source = ImageSource.FromStream(() => { return (Stream)stream; });
                        img = stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }

        private async void Ent_TextChanged(object sender, TextChangedEventArgs e)
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
            catch(Exception ex)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }

        private async void BtnSalva_Clicked(object sender, EventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(entCognome.Text) && !string.IsNullOrEmpty(entNome.Text))
                {
                    AccountClient accountClient = new AccountClient(Api.ApiHelper.GetApiClient());
                    UpdateUserBindingModel updateUserBindingModel = new UpdateUserBindingModel()
                    {
                        Name = entNome.Text,
                        Surname = entCognome.Text,
                        BirthName = entNome.Text,
                        //DataNascita = pkDataNascita.Date,
                        ImmagineProfilo = img,
                        Email = viewModel.Email
                    };

                    //TODO: gestire la modifica della Email.
                    await accountClient.UpdateUserAsync(updateUserBindingModel);

                    //Refresh del menu
                    MenuPage menuPage = (MenuPage)((MasterDetailPage)Application.Current.MainPage).Master;
                    await menuPage.UpdateMenuData(Models.MenuItemType.Account);
                }
                else
                {
                    await DisplayAlert("Attenzione", "è necessario compilare tutti i campi", "OK");
                }
            }
            catch (Exception)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }

        private async void BtnCambiaPassword_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Login.CambiaPassword());
            }
            catch (Exception)
            {
                //Navigo alla pagina d'errore.
                await Navigation.PushAsync(new ErrorPage());
            }
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            PickPhoto();

            (sender as Button).IsEnabled = true;
        }

        async void PickPhoto()
        {
            await CrossMedia.Current.Initialize();

            MediaFile foto = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (foto == null)
                return;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                foto.GetStream().CopyTo(memoryStream);
                viewModel.FotoProfilo = memoryStream.ToArray();
                imgFotoUtente.Source = ImageSource.FromStream(() => { return new MemoryStream(viewModel.FotoProfilo); });
            }
        }
    }
}