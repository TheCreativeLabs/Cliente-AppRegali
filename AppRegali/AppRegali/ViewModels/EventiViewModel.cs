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
        public EventoCategoria Categoria { get; set; }

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        private int CurrentPage = 1;
        private int PageSize = 5;
        EventoClient eventoClient = new EventoClient(Api.ApiHelper.GetApiClient());


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

        async Task LoadMoreCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ICollection<EventoDtoOutput> news = null;
            try
            {
                CurrentPage += 1;

                //news = await GetAnnunci();

                if (news != null)
                {
                    foreach (var item in news)
                    {
                        Items.Add(item);
                        //OnpropertyChanged("Items");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //In questo modo capisco se ci sono ancora record da caricare
                //if(news != null && news.Count == PageSize )
                //{
                IsBusy = false;
                //}
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            //if (IsLoading)
            //    return;

            IsBusy = true;
            //IsLoading = true;

            try
            {
                Items.Clear();


                ICollection<EventoDtoOutput> listaEventi;// = await getAnnunci(null, null);

                if (SoloPersonali)
                {
                    listaEventi = await eventoClient.GetEventoCurrentUserAsync();
                }
                else
                {
                    string idCategoria = null;
                    if(Categoria != null)
                    {
                        idCategoria = Categoria.Id.ToString();
                    }
                    listaEventi = await eventoClient.GetEventiByidUtenteAsync(1, 5, null, idCategoria);
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
                //IsLoading = false;
            }
        }

        //private async Task<ICollection<EventoDtoOutput>> getAnnunci(string idUtente = null, string idCategoria = null) {
        //    ICollection<EventoDtoOutput> listaEventi;
        //    if (SoloPersonali)
        //    {
        //        listaEventi = await eventoClient.GetEventoCurrentUserAsync();
        //    }
        //    else
        //    {
        //        listaEventi = await eventoClient.GetEventiByidUtenteAsync(null, null);
        //    }
        //    return listaEventi;
        //}
    }
}