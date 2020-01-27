using System;
using System.Collections.Generic;
using System.Linq;
using Api;
using AppRegali.Api;
using AppRegali.ViewModels;
using Xamarin.Forms;

namespace AppRegali.Views
{
    public partial class RegaloPersonaleDettaglio : ContentPage
    {
        RegaloDtoOutput Regalo;

        public RegaloPersonaleDettaglio(RegaloDtoOutput RegaloParam)
        {
            InitializeComponent();

            Regalo = RegaloParam;
            BindingContext = Regalo;

            if (Regalo != null && Regalo.ImportoCollezionato.HasValue)
                ValueProgress.Progress = Regalo.ImportoCollezionato.Value / Regalo.Prezzo;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
            PartecipazioneDtoOutput partecipazione = await eventoClient.GetPartecipazioniRegaloAsync(new Guid(Regalo.Id));
            lvPartecipanti.ItemsSource = partecipazione.UtentiPartecipanti.ToList();
            if (partecipazione.NumeroAnonimi.HasValue && partecipazione.NumeroAnonimi.Value > 0)
            {
                NumeroPartecipanti.Text = partecipazione.NumeroAnonimi.ToString();
                LabelAnonimi.IsVisible = true;
            }
        }

        private async void BtnModificaRegalo_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RegaloModifica(new RegaloDetailViewModel(Regalo)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await DisplayAlert("Attenzione", "Vuoi davvero eliminare il regalo?", "Yes", "No");
                if (answer)
                {
                    try
                    {

                        EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
                        await eventoClient.DeleteRegaloAsync(new Guid(Regalo.Id));

                        MessagingCenter.Send(this, "RefreshListaRegaliPersonaliElimina", "OK");

                        await DisplayAlert(null, "Regalo eliminato", "Ok");
                        //torno indietro alla lista degli eventi personali
                        await Navigation.PopAsync();
                        await Navigation.PopModalAsync();
                    }
                    catch
                    {
                        await DisplayAlert(null, "Errore durante l'eliminazione del regalo", "Ok");
                    }
                    finally
                    {
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

       async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet(null, "Annulla", "Elimina", "Modifica");

            if (action == "Elimina")
            {
                bool answer = await DisplayAlert("Attenzione", "Vuoi davvero eliminare il regalo?", "Yes", "No");
                if (answer)
                {
                    try
                    {
                        //RegaloPersonaleDettaglioActivityIndicator.IsVisible = true;

                        EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
                        await eventoClient.DeleteRegaloAsync(new Guid(Regalo.Id));

                        MessagingCenter.Send(this, "RefreshListaRegaliPersonaliElimina", "OK");

                        await DisplayAlert(null, "Regalo eliminato", "Ok");
                        //torno indietro alla lista degli eventi personali
                        await Navigation.PopAsync();
                        await Navigation.PopModalAsync();
                    }
                    catch
                    {
                        await DisplayAlert(null, "Errore durante l'eliminazione del regalo", "Ok");
                    }
                    finally
                    {
                        //RegaloPersonaleDettaglioActivityIndicator.IsVisible = false;
                    }
                }
            }
            else if (action == "Modifica")
            {
                await Navigation.PushAsync(new RegaloModifica(new RegaloDetailViewModel(Regalo)));
            }
        }
    }
}
