using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Api;
using AppRegali.Models;
using AppRegali.Views;
using System.Net.Http;
using System.Collections.Generic;

namespace AppRegali.ViewModels
{
    public class AmiciViewModel : BaseViewModel
    {
        public ObservableCollection<UserInfoDto> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public Command LoadItemsFilteredCommand { get; set; }

        private bool SoloAccettati { get; set; }

        //private string Filter { get; set; }


        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }


        public AmiciViewModel(bool SoloAmiciAccettati)
        {
            Items = new ObservableCollection<UserInfoDto>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadItemsFilteredCommand = new Command(async (object filter) => await ExecuteLoadItemsCommandFiltered((string) filter));
            SoloAccettati = SoloAmiciAccettati;
            //Filter = filterString;
        }


        //se amiciAccettati == true -> restituisce gli utenti che sono MIEI AMICI (accettato = true)
        //se amiciAccettati == false -> restituisce gli utenti che HANNO CHIESTO di essere miei amici (amicizie di cui il current è destinario, con accettato = false)
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (IsLoading)
                return;

            IsLoading = true;

            try
            {
                Items.Clear();

                AmiciClient amiciClient = new AmiciClient(Api.ApiHelper.GetApiClient());

                ICollection<UserInfoDto> listaUsers;

                if (SoloAccettati)
                {
                    listaUsers = await amiciClient.GetAmiciOfCurrentUserAsync();
                }
                else
                {
                    listaUsers = await amiciClient.GetRichiesteAmiciziaOfCurrentUserAsync();
                }

                foreach (var user in listaUsers)
                {
                    Items.Add(user);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                IsLoading = false;
            }
        }

        //restituisce gli utenti che hanno corrispondenza con il filtro (nome cognome contiene il filtro)
        async Task ExecuteLoadItemsCommandFiltered(string Filter)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (IsLoading)
                return;

            IsLoading = true;

            try
            {
                Items.Clear();

                AmiciClient amiciClient = new AmiciClient(Api.ApiHelper.GetApiClient());

                ICollection<UserInfoDto> listaUsers;

                if(Filter != null && Filter.Length != 0)
                {
                    listaUsers = await amiciClient.GetRicercaUtentiAsync(Filter);
                    
                    foreach (var user in listaUsers)
                    {
                        Items.Add(user);
                    }
                } 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                IsLoading = false;
            }
        }
    }
}