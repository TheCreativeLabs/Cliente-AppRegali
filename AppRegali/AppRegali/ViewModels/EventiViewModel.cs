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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppRegali.ViewModels
{
    public class EventiViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<EventoDtoOutput> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command LoadMore { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public EventoCategoria Categoria { get; set; }
        public Guid? IdUtente { get; set; }

        public int CurrentPage { get; set; }
        private int PageSize = 5;

        EventoClient eventoClient = new EventoClient(Api.ApiHelper.GetApiClient());

        private bool SoloPersonali { get; set; }

        public EventiViewModel(bool SoloEventiPersonali)
        {
            Items = new ObservableCollection<EventoDtoOutput>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SoloPersonali = SoloEventiPersonali;
            OnpropertyChanged("Items");
            CurrentPage = 1;

            this.LoadMore = new Command(async () => await LoadMoreCommand());

            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    await DataStore.AddItemAsync(newItem);
            //});
        }

        void OnpropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        async Task LoadMoreCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            OnpropertyChanged("IsBusy");

            ICollection<EventoDtoOutput> news = null;
            try
            {
                CurrentPage += 1;

                news = await GetEventi();

                if (news != null)
                {
                    foreach (var item in news)
                    {
                        Items.Add(item);
                        OnpropertyChanged("Items");
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
                OnpropertyChanged("IsBusy");

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
            OnpropertyChanged("IsBusy");

            //IsLoading = true;

            try
            {
                Items.Clear();


                ICollection<EventoDtoOutput> listaEventi;

                if (SoloPersonali)
                {
                    listaEventi = await GetEventi();
                }
                else
                {

                    listaEventi = await GetEventi();
                }

                foreach (var evento in listaEventi)
                {
                    Items.Add(evento);
                }

                OnpropertyChanged("Items");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                OnpropertyChanged("IsBusy");
            }
        }

        private async Task<ICollection<EventoDtoOutput>> GetEventi() {
            ICollection<EventoDtoOutput> listaEventi;
            if (SoloPersonali)
            {
                listaEventi = await eventoClient.GetEventoCurrentUserAsync();
            }
            else
            {
                string idCategoria = null;
                if (Categoria != null)
                {
                    idCategoria = Categoria.Id.ToString();
                }

                string idUser = null;

                if (IdUtente.HasValue)
                    idUser = IdUtente.Value.ToString();

                listaEventi = await eventoClient.GetEventiByidUtenteAsync(CurrentPage, PageSize, idUser, idCategoria);
            }
            return listaEventi;
        }
    }
}