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
using AppRegali.Api;
using System.Linq;

namespace AppRegali.ViewModels
{
    public class DesideriViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<DesiderioDto> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public bool IsEmpty { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public DesideriViewModel()
        {
            Items = new ObservableCollection<DesiderioDto>();
            OnpropertyChanged("Items");
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        void OnpropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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

            IsEmpty = false;
            OnpropertyChanged("IsEmpty");


            //IsLoading = true;

            try
            {
                Items.Clear();


                ICollection<DesiderioDto> listaDesideri;

                DesideriClient desideriClient = new DesideriClient(await ApiHelper.GetApiClient());
                listaDesideri = await desideriClient.GetDesideriCurrentUserAsync();

                if (!listaDesideri.Any())
                {
                    IsEmpty = true;
                    OnpropertyChanged("IsEmpty");
                }

                foreach (var evento in listaDesideri)
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

    }
}