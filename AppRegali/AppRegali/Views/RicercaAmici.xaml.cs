using Api;
using AppRegali.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RicercaAmici : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public Command CommandLblCancel_Tapped { get; set; }

        public AmiciViewModel viewModel;


        public RicercaAmici()
        {
            InitializeComponent();

            entRicerca.ReturnCommand = new Command(() => MyDisplayAlert());

            //entRicerca.Focus();



            BindingContext = viewModel = new AmiciViewModel(false);
        }

        protected override void OnAppearing()
        {
            entRicerca.Focus();
        }

        public async void lblCancel_Tapped(object sender, EventArgs e) 
        {
            try
            {
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
                //Navigo alla pagina d'errore.
                Application.Current.MainPage = new ErrorPage();
            }
        }

        public async void MyDisplayAlert()
        {
            viewModel.LoadItemsFilteredCommand.Execute(entRicerca.Text);
            //RichiesteCollectionView.IsVisible = true;
            //await DisplayAlert("aa", "bb", "cc");
        }

        async void btnCerca_Clicked(object sender, EventArgs e)
        {
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        async void OnPeopleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserInfoDto current = (e.CurrentSelection.FirstOrDefault() as UserInfoDto);

            if (current == null || current.IdAspNetUser == null)
                return;

            await Navigation.PushAsync(new AmiciProfilo(current));
        }
    }
}
