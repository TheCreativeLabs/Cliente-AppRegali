//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Threading.Tasks;
//using Xamarin.Forms;
//using Xamarin.Forms.Xaml;

//using AppRegali.Models;
//using Api;

//namespace AppRegali.Views
//{
//    // Learn more about making custom code visible in the Xamarin.Forms previewer
//    // by visiting https://aka.ms/xamarinforms-previewer
//    [DesignTimeVisible(false)]
//    public partial class MainPage : MasterDetailPage
//    {
//        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
//        public MainPage()
//        {
//            InitializeComponent();

//            MasterBehavior = MasterBehavior.Popover;

//            MenuPages.Add((int)MenuItemType.Home, (NavigationPage)Detail);
//        }

//        public async Task NavigateFromMenu(int id, object Model)
//        {
//            if (!MenuPages.ContainsKey(id))
//            {
//                switch (id)
//                {
//                    case (int)MenuItemType.Home:
//                        MenuPages.Add(id, new NavigationPage(new Home()));
//                        break;
//                    case (int)MenuItemType.EventiPersonali:
//                        MenuPages.Add(id, new NavigationPage(new EventiPersonali()));
//                        break;
//                    case (int)MenuItemType.Amici:
//                        MenuPages.Add(id, new NavigationPage(new Amici()));
//                        break;
//                    case (int)MenuItemType.Account:
//                        MenuPages.Add(id, new NavigationPage(new Account.Account((UserInfoDto)Model)));
//                        break;
//                }
//            }

//            var newPage = MenuPages[id];

//            if (newPage != null && Detail != newPage)
//            {
//                Detail = newPage;

//                //if (Device.RuntimePlatform == Device.Android)
//                //    await Task.Delay(100);

//                IsPresented = false;
//            }
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRegali.Api;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.Children.Add(new ContentPage
            {
                Title = "Text",
                Content = new StackLayout
                {
                    Padding = 20,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children =
                {
                    new Image
                    {
                        Source = ImageSource.FromFile("monkeyicon.png")
                    },
                    new Label
                    {
                        TextColor = Color.FromHex("#5F5A5A"),
                        FontSize = 16,
                        Text = "Other text"
                    }
             }
                }
            });
            this.Children.Add(new ContentPage
            {
                Title = "Button",
                Content = new StackLayout
                {
                    Children ={
              new Xamarin.Forms.Button{
                ImageSource = ImageSource.FromFile("logobiancobluombra.png"),
                BackgroundColor = Color.Transparent

                }
             }
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
    }
}