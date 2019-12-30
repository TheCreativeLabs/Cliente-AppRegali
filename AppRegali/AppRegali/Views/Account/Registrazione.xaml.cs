using Api;
using DependencyServiceDemos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registrazione : ContentPage
    {

        byte[] img;


        public Registrazione()
        {
            InitializeComponent();
        }

        private async void btnRegistrati_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool formIsValid = true;

                //Controllo validità del nome.
                if (String.IsNullOrEmpty(entNome.Text))
                {
                    formIsValid = false;
                    //lblValidatorEntNome.IsVisible = true;
                }

                //Controllo validità del cognome.
                if (String.IsNullOrEmpty(entCognome.Text))
                {
                    formIsValid = false;
                    //lblValidatorEntCognome.IsVisible = true;
                }

                if (dpDataNascita.Date == null)
                {
                    formIsValid = false;
                }

                //Controllo validità della mail.
                if (String.IsNullOrEmpty(entEmail.Text) || !Regex.IsMatch(entEmail.Text, Utility.Utility.EmailRegex))
                {
                    formIsValid = false;
                    //entEmail.BackgroundColor = Color.FromRgb(255, 175, 173);
                    //lblValidatorEntEmail.IsVisible = true;
                }

                //Controllo validità della password.
                if (String.IsNullOrEmpty(entPassword.Text) ||  !Regex.IsMatch(entPassword.Text, Utility.Utility.PasswordRegex))
                {
                    formIsValid = false;
                    //entPassword.BackgroundColor = Color.FromRgb(255, 175, 173);
                    //lblValidatorEntPassword.IsVisible = true;
                }

                //Controllo che le due password siano uguali.
                if (String.IsNullOrEmpty(entConfermaPassword.Text) || !(entPassword.Text == entConfermaPassword.Text))
                {
                    formIsValid = false;
                    //entConfermaPassword.BackgroundColor = Color.FromRgb(255, 175, 173);
                    //lblValidatorEntConfermaPassword.IsVisible = true;
                }

                //Se la form è valida proseguo con la registrazione.
                if (formIsValid)
                {

                    HttpClient httpClient = new HttpClient();
                    AccountClient accountClient = new AccountClient(httpClient);

                        //Creo il modello dei dati per la registrazione
                        RegisterUserBindingModel registerBindingModel = new RegisterUserBindingModel()
                        {
                            Name = entNome.Text,
                            Surname = entCognome.Text,
                            BirthName = entNome.Text,
                            ImmagineProfilo = img,
                            DataNascita = dpDataNascita.Date,
                            Email = entEmail.Text,
                            Password = entPassword.Text,
                            ConfirmPassword = entConfermaPassword.Text
                        };

                        //TODO: gestire la data di nascita

                        await accountClient.RegisterAsync(registerBindingModel);

                        await DisplayAlert("Registrazione avvenuta!", "Riceverai una email per confermare il tuo account", "OK");

                        await Navigation.PopModalAsync();
                }
                else
                {
                  await DisplayAlert("Attenzione", "è necessario compilare tutti i campi", "OK");
                }
            }
            catch (Exception ex)
            {
                //TODO Gestire l'errore navigando alla pagina specifica.
            }
        }
        private void btnLogin_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Login());
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void ent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Controllo che username e password siano valorizzati.
                if (!(String.IsNullOrEmpty(entNome.Text)) && !(String.IsNullOrEmpty(entCognome.Text))
                    && !(String.IsNullOrEmpty(entPassword.Text)) && !(String.IsNullOrEmpty(entConfermaPassword.Text)) && !(String.IsNullOrEmpty(entEmail.Text)))
                {
                    btnRegistrati.IsEnabled = true;
                }
                else
                {
                    btnRegistrati.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                MemoryStream i = new MemoryStream();
                stream.CopyTo(i);
                img = i.ToArray();
                imgFotoUtente.Source = ImageSource.FromStream(() => stream);

                //using (var memoryStream = new MemoryStream())
                //{
                //    stream.CopyTo(memoryStream);
                //    img = memoryStream.ToArray();
                //}
            }

            (sender as Button).IsEnabled = true;

        }
    }
}