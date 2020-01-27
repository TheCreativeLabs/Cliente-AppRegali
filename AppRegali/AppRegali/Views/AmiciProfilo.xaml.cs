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

            this.Title = $"{user.Nome} {user.Cognome}";
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
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext =
            viewModel = await UserProfiloViewModel.ExecuteLoadCommandAsync(User.IdAspNetUser);

            //ricarico ogni volta per recepire le modifiche
            viewModel.LoadItemsCommand.Execute(null);

            EventiListView.ItemsSource = viewModel.Eventi;

            if (viewModel.Info.Relation.Value == "CONTACT")
            {
                btnConfirmDenyContact.IsVisible = false;
                SettingMenu.IsVisible = true;
                //viewModel.BtnDeleteContactVisible = true;
            }
            else if (viewModel.Info.Relation.Value == "REQUEST_IN")
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
        }

        private async void btnSendRequest_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

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
            string action = await DisplayActionSheet("Che cosa vuoi fare?", "Annulla", null, "Rimuovi amico");

            if (action == "Rimuovi amico")
            {
                bool answer = await DisplayAlert("Attenzione",
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.ConfirmDelete", CurrentCulture.Ci),
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Yes", CurrentCulture.Ci),
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.No", CurrentCulture.Ci));
                    "Vuoi procedre con l'eliminazione?",
                    "Sì",
                    "No");

                if (answer)
                {
                    AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

                    try
                    {
                        await amiciClient.AmiciziaDeleteOrDenyAsync(viewModel.Info.IdAspNetUser);
                        btnConfirmDenyContact.IsVisible = false;
                        btnConfirmDenyContact.IsVisible = false;
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

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as EventoDtoOutput;

            if (item == null || item.Id == null)
                return;

            await Navigation.PushAsync(new EventoDettaglio(item));
        }

        private async void btnConfirmContact_Clicked(object sender, EventArgs e)
        {
            AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

            try
            {
                await amiciClient.AccettaAmiciziaAsync(viewModel.Info.IdAspNetUser.ToString());
                btnConfirmDenyContact.IsVisible = false;
                //btnContactSetting.IsVisible = true;
                SettingMenu.IsVisible = true;
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
                btnConfirmDenyContact.IsVisible = true;
                btnConfirmDenyContact.IsVisible = false;
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

        async void btnBack_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch
            {
                try
                {
                    await Navigation.PopAsync();
                }
                catch
                {
                    await Navigation.PushAsync(new ErrorPage());
                }
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as EventoDtoOutput;
            if (item == null || item.Id == null)
                return;

            await Navigation.PushAsync(new EventoDettaglio(item));

            // Manually deselect item.
            EventiListView.SelectedItem = null;
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet("Che cosa vuoi fare?", "Annulla", null, "Rimuovi amico");

            if (action == "Rimuovi amico")
            {
                bool answer = await DisplayAlert("Attenzione",
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.ConfirmDelete", CurrentCulture.Ci),
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Yes", CurrentCulture.Ci),
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.No", CurrentCulture.Ci));
                    "Vuoi procedre con l'eliminazione?",
                    "Sì",
                    "No");

                if (answer)
                {
                    AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

                    try
                    {
                        await amiciClient.AmiciziaDeleteOrDenyAsync(viewModel.Info.IdAspNetUser);
                        btnConfirmDenyContact.IsVisible = false;
                        btnConfirmDenyContact.IsVisible = false;
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

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet("Che cosa vuoi fare?", "Annulla", null, "Rimuovi amico");

            if (action == "Rimuovi amico")
            {
                bool answer = await DisplayAlert("Attenzione",
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.ConfirmDelete", CurrentCulture.Ci),
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.Yes", CurrentCulture.Ci),
                    //Helpers.TranslateExtension.ResMgr.Value.GetString("EventoModifica.No", CurrentCulture.Ci));
                    "Vuoi procedre con l'eliminazione?",
                    "Sì",
                    "No");

                if (answer)
                {
                    AmiciClient amiciClient = new AmiciClient(await Api.ApiHelper.GetApiClient());

                    try
                    {
                        await amiciClient.AmiciziaDeleteOrDenyAsync(viewModel.Info.IdAspNetUser);
                        btnConfirmDenyContact.IsVisible = false;
                        btnConfirmDenyContact.IsVisible = false;
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
    }
}