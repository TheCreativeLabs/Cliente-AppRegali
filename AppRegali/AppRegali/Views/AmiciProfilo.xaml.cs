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
        static Helpers.TranslateExtension translate = new Helpers.TranslateExtension();
        Guid IdAmico;

        public AmiciProfilo(Guid IdUser)
        {
            InitializeComponent();

            IdAmico = IdUser;
            aiLoading.IsVisible = true;
            aiLoading.IsRunning = true;
            //BindingContext = viewModel = new UserProfiloViewModel() { Nome = "provvisorio" };
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = viewModel = await UserProfiloViewModel.ExecuteLoadCommandAsync(IdAmico);

            //ricarico ogni volta per recepire le modifiche
            viewModel.LoadItemsCommand.Execute(null); 

            if (viewModel.Info.Relation.Value == "CONTACT")
            {
                btnDeleteContact.IsVisible = true;
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

            frmImgEventoDett.IsVisible = true;

            aiLoading.IsVisible = false;
            aiLoading.IsRunning = false;
        }

        private async void btnSendRequest_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(Api.ApiHelper.GetApiClient());

            try 
            { 
                await amiciClient.InserisciAmiciziaAsync(viewModel.Info.IdAspNetUser.ToString());
                btnSendRequest.IsVisible = false;
                btnRequestSent.IsVisible = true;
            }
            catch
            {
                await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorSendRequest", translate.ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", translate.ci));
            }
            
        }

        private async void btnConfirmContact_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(Api.ApiHelper.GetApiClient());

            try
            {
                await amiciClient.AccettaAmiciziaAsync(viewModel.Info.IdAspNetUser.ToString());
                btnConfirmDenyContact.IsVisible = false;
                btnDeleteContact.IsVisible = true;
            }
            catch
            {
                await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorGeneric", translate.ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", translate.ci));
            }

        }

        private async void btnDeleteContact_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(Api.ApiHelper.GetApiClient());

            try
            {
                await amiciClient.AmiciziaDeleteOrDenyAsync(viewModel.Info.IdAspNetUser);
                btnConfirmDenyContact.IsVisible = false;
                btnDeleteContact.IsVisible = false;
                btnSendRequest.IsVisible = true;
            }
            catch
            {
                await DisplayAlert(null,
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.ErrorGeneric", translate.ci),
                            Helpers.TranslateExtension.ResMgr.Value.GetString("AmiciProfilo.Ok", translate.ci));
            }

        }
    }
}