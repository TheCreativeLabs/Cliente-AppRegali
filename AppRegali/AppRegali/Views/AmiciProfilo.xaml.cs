using Api;
using AppRegali.Utility;
using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
        //static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        UserInfoDto User;


        public AmiciProfilo(UserInfoDto user)
        {
            InitializeComponent();

            Nome.Text = user.Nome;
            Cognome.Text = user.Cognome;

            if (user.DataDiNascita.HasValue)
            {
                DataNascita.Text = user.DataDiNascita.Value.ToString("dd/MM/yyyy");
            }

            if (user.FotoProfilo != null)
            {
                Stream stream = new MemoryStream(user.FotoProfilo);
                imgProfilo.Source = ImageSource.FromStream(() => { return stream; });
            }
            else if (user.PhotoUrl != null)
            {
                imgProfilo.Source = ImageSource.FromUri(new Uri(user.PhotoUrl));
            }

            User = user;
            aiLoading.IsVisible = true;
            aiLoading.IsRunning = true;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //BindingContext =
            viewModel = await UserProfiloViewModel.ExecuteLoadCommandAsync(User.IdAspNetUser);
            
            //ricarico ogni volta per recepire le modifiche
            viewModel.LoadItemsCommand.Execute(null);

            EventiProfiloListView.ItemsSource = viewModel.Eventi;

            if (viewModel.Info.Relation.Value == "CONTACT")
            {
                btnContactSetting.IsVisible = true;
                //viewModel.BtnDeleteContactVisible = true;
            } else if (viewModel.Info.Relation.Value == "REQUEST_IN")
            {
                btnConfirmDenyContact.IsVisible = true;
            }
            else if (viewModel.Info.Relation.Value == "REQUEST_OUT")
            {
                btnRequestSent.IsVisible = true;
            }
            else if (viewModel.Info.Relation.Value == "STRANGER")
            {
                btnSendRequest.IsVisible = true;
            }

            aiLoading.IsVisible = false;
            aiLoading.IsRunning = false;
        }

        private async void btnSendRequest_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(await  Api.ApiHelper.GetApiClient());

            try 
            { 
                await amiciClient.InserisciAmiciziaAsync(viewModel.Info.IdAspNetUser.ToString());
                btnSendRequest.IsVisible = false;
                btnRequestSent.IsVisible = true;
            }
            catch
            {
                await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorSendRequest", CurrentCulture.Ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", CurrentCulture.Ci));
            }
            
        }

        private async void UserSetting_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, "Rimuovi amico");

            if(action == "Rimuovi amico")
            {
                AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

                try
                {
                    await amiciClient.AmiciziaDeleteOrDenyAsync(viewModel.Info.IdAspNetUser);
                    btnConfirmDenyContact.IsVisible = false;
                    btnContactSetting.IsVisible = false;
                    btnSendRequest.IsVisible = true;
                    viewModel.LoadItemsCommand.Execute(null);
                }
                catch
                {
                    await DisplayAlert(null,
                                Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorGeneric", CurrentCulture.Ci),
                                Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", CurrentCulture.Ci));
                }
            }
        }

        private async void btnConfirmContact_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

            try
            {
                await amiciClient.AccettaAmiciziaAsync(viewModel.Info.IdAspNetUser.ToString());
                btnConfirmDenyContact.IsVisible = false;
                btnContactSetting.IsVisible = true;
                viewModel.LoadItemsCommand.Execute(null);
            }
            catch
            {
                await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorGeneric", CurrentCulture.Ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", CurrentCulture.Ci));
            }

        }
        
          private async void btnDeleteContact_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

            try
            {
                await amiciClient.AmiciziaDeleteOrDenyAsync(viewModel.Info.IdAspNetUser);
                btnConfirmDenyContact.IsVisible = false;
                btnContactSetting.IsVisible = false;
                btnSendRequest.IsVisible = true;
                viewModel.LoadItemsCommand.Execute(null);
            }
            catch
            {
                await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorGeneric", CurrentCulture.Ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", CurrentCulture.Ci));
            }

        }
    }
}