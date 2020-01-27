using Api;
using AppRegali.Api;
using System;
using System.Collections.Generic;
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
    public partial class CambiaPassword : ContentPage
    {
        public CambiaPassword()
        {
            InitializeComponent();
        }

        //private async void btnProcedi_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        bool formIsValid = true;

        //        //Controllo validità della vecchia password.
        //        if (String.IsNullOrEmpty(entVecchiaPassword.Text))
        //        {
        //            formIsValid = false;
        //        }

        //        //Controllo validità della nuova password.
        //        if (String.IsNullOrEmpty(entNuovaPassword.Text) || !Regex.IsMatch(entNuovaPassword.Text, Utility.Utility.PasswordRegex))
        //        {
        //            formIsValid = false;
        //            lblValidatorEntPassword.IsVisible = true;
        //            entNuovaPassword.BackgroundColor = Color.FromRgb(255, 175, 173);

        //        }

        //        //Controllo che le due password siano uguali.
        //        if (String.IsNullOrEmpty(entConfermaPassword.Text) || !(entNuovaPassword.Text == entConfermaPassword.Text))
        //        {
        //            formIsValid = false;
        //            lblValidatorEntConfermaPassword.IsVisible = true;
        //            entConfermaPassword.BackgroundColor = Color.FromRgb(255, 175, 173);
        //        }

        //        //Se la form è valida proseguo con la registrazione.
        //        if (formIsValid)
        //        {
        //            lblValidatorEntConfermaPassword.IsVisible = false;
        //            entNuovaPassword.BackgroundColor = Color.White;
        //            entConfermaPassword.BackgroundColor = Color.White;
        //            AccountClient accountClient = new AccountClient(await ApiHelper.GetApiClient());

        //            ChangePasswordBindingModel changePasswordBindingModel = new ChangePasswordBindingModel()
        //            {
        //                OldPassword = entVecchiaPassword.Text,
        //                NewPassword = entNuovaPassword.Text,
        //                ConfirmPassword = entConfermaPassword.Text
        //            };

        //            ChangePasswordLoading.IsVisible = true;
        //            await accountClient.ChangePasswordAsync(changePasswordBindingModel);
        //            ChangePasswordLoading.IsVisible = false;
        //            await DisplayAlert(null, "Password cambiata correttamente", "OK");
        //        }
        //    }
        //    catch (ApiException Ex)
        //    {
        //        //Se sono qui non ho l'accesso, quindi la password è sbagliata.
        //        await DisplayAlert("Attenzione", "La password inserita non è valida.", "OK");
        //    }
        //    catch (Exception)
        //    {
        //        await Navigation.PushAsync(new ErrorPage());
        //    }
        //}

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool formIsValid = true;

                //Controllo validità della vecchia password.
                if (String.IsNullOrEmpty(entVecchiaPassword.Text))
                {
                    formIsValid = false;
                    await DisplayAlert("Attenzione", "La vecch password non è valida.", "OK");
                    return;
                }

                //Controllo validità della nuova password.
                if (String.IsNullOrEmpty(entNuovaPassword.Text) || !Regex.IsMatch(entNuovaPassword.Text, Utility.Utility.PasswordRegex))
                {
                    formIsValid = false;
                    await DisplayAlert("Attenzione", "La nuova password non è valida. Deve contenere almeno un carattere specialeun numero e una lettera maiuscola.", "OK");
                    return;

                }

                //Controllo che le due password siano uguali.
                if (String.IsNullOrEmpty(entConfermaPassword.Text) || !(entNuovaPassword.Text == entConfermaPassword.Text))
                {
                    formIsValid = false;
                    await DisplayAlert("Attenzione", "Le due password non corrispondono.", "OK");
                    return;
                }

                //Se la form è valida proseguo con la registrazione.
                if (formIsValid)
                {
                    entNuovaPassword.BackgroundColor = Color.White;
                    entConfermaPassword.BackgroundColor = Color.White;
                    AccountClient accountClient = new AccountClient(await ApiHelper.GetApiClient());

                    ChangePasswordBindingModel changePasswordBindingModel = new ChangePasswordBindingModel()
                    {
                        OldPassword = entVecchiaPassword.Text,
                        NewPassword = entNuovaPassword.Text,
                        ConfirmPassword = entConfermaPassword.Text
                    };

                    ChangePasswordLoading.IsVisible = true;
                    await accountClient.ChangePasswordAsync(changePasswordBindingModel);
                    ChangePasswordLoading.IsVisible = false;
                    await DisplayAlert(null, "Password cambiata correttamente", "OK");

                }
            }
            catch (ApiException Ex)
            {
                //Se sono qui non ho l'accesso, quindi la password è sbagliata.
                await DisplayAlert("Attenzione", "La password inserita non è valida.", "OK");
            }
            catch (Exception)
            {
                await Navigation.PushAsync(new ErrorPage());
            }
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool formIsValid = true;

                //Controllo validità della vecchia password.
                if (String.IsNullOrEmpty(entVecchiaPassword.Text))
                {
                    formIsValid = false;
                    await DisplayAlert("Attenzione", "La vecch password non è valida.", "OK");
                    return;
                }

                //Controllo validità della nuova password.
                if (String.IsNullOrEmpty(entNuovaPassword.Text) || !Regex.IsMatch(entNuovaPassword.Text, Utility.Utility.PasswordRegex))
                {
                    formIsValid = false;
                    await DisplayAlert("Attenzione", "La nuova password non è valida. Deve contenere almeno un carattere specialeun numero e una lettera maiuscola.", "OK");
                    return;

                }

                //Controllo che le due password siano uguali.
                if (String.IsNullOrEmpty(entConfermaPassword.Text) || !(entNuovaPassword.Text == entConfermaPassword.Text))
                {
                    formIsValid = false;
                    await DisplayAlert("Attenzione", "Le due password non corrispondono.", "OK");
                    return;
                }

                //Se la form è valida proseguo con la registrazione.
                if (formIsValid)
                {
                    entNuovaPassword.BackgroundColor = Color.White;
                    entConfermaPassword.BackgroundColor = Color.White;
                    AccountClient accountClient = new AccountClient(await ApiHelper.GetApiClient());

                    ChangePasswordBindingModel changePasswordBindingModel = new ChangePasswordBindingModel()
                    {
                        OldPassword = entVecchiaPassword.Text,
                        NewPassword = entNuovaPassword.Text,
                        ConfirmPassword = entConfermaPassword.Text
                    };

                    ChangePasswordLoading.IsVisible = true;
                    await accountClient.ChangePasswordAsync(changePasswordBindingModel);
                    ChangePasswordLoading.IsVisible = false;
                    await DisplayAlert(null, "Password cambiata correttamente", "OK");

                }
            }
            catch (ApiException Ex)
            {
                //Se sono qui non ho l'accesso, quindi la password è sbagliata.
                await DisplayAlert("Attenzione", "La password inserita non è valida.", "OK");
            }
            catch (Exception)
            {
                await Navigation.PushAsync(new ErrorPage());
            }
        }
    }
}