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
        public String Nome { get; set; }
        public ObservableCollection<EventoDtoOutput> Eventi { get; set; }
        public Command LoadItemsCommand { get; set; }
        private HttpClient httpClient { get; set; }
    public UserProfiloViewModel(UserInfoDto userInfo)
        {
            this.httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());
            Info = new UserInfoDto();
            Eventi = new ObservableCollection<EventoDtoOutput>();
            this.UserId = userInfo.IdAspNetUser;

             Info = userInfo;
             Nome = userInfo.Nome;

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

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
