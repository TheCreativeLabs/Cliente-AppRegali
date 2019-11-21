using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using AppRegali.Models;
using Xamarin.Forms;

namespace AppRegali.ViewModels
{
    public class UserProfiloViewModel : BaseViewModel
    {
        public Guid UserId { get; set; }
        public UserInfoDto Info { get; set; }
        //public String nome { get; set; }

        private string nome;
        public string Nome
        {
            get { return nome; }
            set
            {
                nome = value;
                OnPropertyChanged(nameof(Nome)); // Notify that there was a change on this property
            }
        }


        public ObservableCollection<EventoDtoOutput> Eventi { get; set; }
        public Command LoadItemsCommand { get; set; }
        private HttpClient httpClient { get; set; }

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        bool btnDeleteContactVisible = false;
        public bool BtnDeleteContactVisible
        {
            get { return btnDeleteContactVisible && !isLoading; }
            set { SetProperty(ref btnDeleteContactVisible, value); }
        }

        public UserProfiloViewModel()
        {
            this.httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());
            Info = new UserInfoDto();
            Eventi = new ObservableCollection<EventoDtoOutput>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

        }

        public static async Task<UserProfiloViewModel> ExecuteLoadCommandAsync(Guid Id)
        {
            //if (IsLoading)
            //    return;

            //IsLoading = true;

            UserProfiloViewModel userProfiloViewModel = new UserProfiloViewModel();
            userProfiloViewModel.UserId = Id;

            //Info = userInfo;
            //Nome = userInfo.Nome;

            try
            {
                AmiciClient amiciClient = new AmiciClient(Api.ApiHelper.GetApiClient());

                userProfiloViewModel.Info = await amiciClient.GetUserInfoByIdUsersAsync(Id);
                userProfiloViewModel.Nome = userProfiloViewModel.Info.Nome;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //await ExecuteLoadItemsCommand();

            //IsLoading = false;

            return userProfiloViewModel;
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Eventi.Clear();
                ICollection<EventoDtoOutput> listaEventi;

                //HttpClient httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());
                //AmiciClient amiciClient = new AmiciClient(httpClient);
                //UserInfoDto userInfo = await amiciClient.GetUserInfoByIdUsersAsync(UserId);

                EventoClient eventoClient = new EventoClient(httpClient);
                listaEventi = await eventoClient.GetEventiByidUtenteAsync(UserId.ToString(), null);

               

                foreach (var evento in listaEventi)
                {
                    Eventi.Add(evento);
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
