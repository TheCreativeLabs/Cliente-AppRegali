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
using System.Linq;
using AppRegali.Api;

namespace AppRegali.ViewModels
{
    public class CategorieViewModel : BaseViewModel
    {
        public ObservableCollection<EventoCategoria> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public CategorieViewModel()
        {
            Items = new ObservableCollection<EventoCategoria>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();

                EventoClient eventoClient = new EventoClient(await ApiHelper.GetApiClient());
                List<EventoCategoria> listaCategorie = (List<EventoCategoria>) await eventoClient.GetLookupEventoCategoriaAsync();

                foreach (var categoria in listaCategorie)
                {
                    Items.Add(categoria);
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