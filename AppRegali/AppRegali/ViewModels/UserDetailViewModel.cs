using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using AppRegali.Api;
using AppRegali.Models;
using Xamarin.Forms;

namespace AppRegali.ViewModels
{
    public class UserDetailViewModel : BaseViewModel
    {
        public UserInfoDto Item { get; set; }
        public Command LoadItemsCommand { get; set; }
        public UserDetailViewModel()
        {
            Item = new UserInfoDto();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

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

                //HttpClient httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Api.ApiHelper.GetToken());
                //AmiciClient amiciClient = new AmiciClient(httpClient);
                UserInfoDto userInfo = await ApiHelper.GetUserInfo();// await amiciClient.GetCurrentUserInfoAsync();

                Item = userInfo;

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
