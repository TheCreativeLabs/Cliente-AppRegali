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
    public class EventiViewModel : BaseViewModel
    {
        public ObservableCollection<EventoDtoOutput> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        private bool SoloPersonali { get; set; }

        public EventiViewModel(bool SoloEventiPersonali)
        {
            Items = new ObservableCollection<EventoDtoOutput>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SoloPersonali = SoloEventiPersonali;
            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    await DataStore.AddItemAsync(newItem);
            //});
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();

                EventoClient eventoClient = new EventoClient(Api.ApiHelper.GetApiClient());

                ICollection<EventoDtoOutput> listaEventi;

                if (SoloPersonali)
                {
                    listaEventi = await eventoClient.GetEventoCurrentUserAsync();
                }
                else
                {
                    listaEventi = await eventoClient.GetEventiByidUtenteAsync(null, null);
                }

                foreach (var evento in listaEventi)
                {
                    Items.Add(evento);
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