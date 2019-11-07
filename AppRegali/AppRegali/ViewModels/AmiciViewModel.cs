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

        private bool SoloAccettati { get; set; }

        //se amiciAccettati == true -> restituisce gli utenti che sono MIEI AMICI (accettato = true)
        //se amiciAccettati == false -> restituisce gli utenti che HANNO CHIESTO di essere miei amici (amicizie di cui il current è destinario, con accettato = false)
        public AmiciViewModel(bool SoloAmiciAccettati)
        {
            Items = new ObservableCollection<UserInfoDto>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SoloAccettati = SoloAmiciAccettati;
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

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
                    //TODO
                    listaUsers = new List<UserInfoDto>();
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
            }
        }
    }
}