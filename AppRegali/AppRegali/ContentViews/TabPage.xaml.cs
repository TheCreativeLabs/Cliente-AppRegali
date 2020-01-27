using System;
using System.Collections.Generic;
using AppRegali.Views;
using AppRegali.Views.Account;
using Xamarin.Forms;
using Sharpnado.Presentation.Forms.CustomViews.Tabs;

namespace AppRegali.ContentViews
{
    public partial class CustomTabPage : ContentView
    {
        public class CustomTabPageModel
        {
            public Color Home { get; set; }
            public Color Desire { get; set; }
            public Color Account { get; set; }
            public Color Setting { get; set; }

        }

        CustomTabPageModel customTabPageModel = new CustomTabPageModel();

        public static readonly BindableProperty CurrentIndexProperty = BindableProperty.Create(
        "CurrentIndex",
        typeof(int),
        typeof(CustomTabPage),
        0,
        BindingMode.Default,
        propertyChanged: DescriptionTextChangedAsync
        );


        public int CurrentIndex
        {
            get => (int)GetValue(CustomTabPage.CurrentIndexProperty);
            set => SetValue(CustomTabPage.CurrentIndexProperty, value);
        }

        private static void DescriptionTextChangedAsync(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomTabPage)bindable;
            control.HomeTab.UnselectedIconColor = Color.Gray;
            control.GiftTab.UnselectedIconColor = Color.Gray;
            control.AccountTab.UnselectedIconColor = Color.Gray;
            control.SettingTab.UnselectedIconColor = Color.Gray;

            switch ((int)newValue)
            {
                case 1:
                    control.HomeTab.UnselectedIconColor = Color.Black;
                    break;

                case 2:
                    control.GiftTab.UnselectedIconColor = Color.Black;
                    break;

                case 3:
                    control.AccountTab.UnselectedIconColor = Color.Black;
                    break;

                case 4:
                    control.SettingTab.UnselectedIconColor = Color.Black;
                    break;
            }

            control.CurrentIndex = (int)newValue;
        }

        public CustomTabPage()
        {
            InitializeComponent();
        }

        async void Button_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new AddPage());
        }

        async void Home_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new Home()));
        }

        async void Desire_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new Desideri()));
        }

        async void Account_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new EventiPersonali()));

        }

        async void Setting_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new Impostazioni()));

        }

       async void TabHost_SelectedTabIndexChanged(System.Object sender, Xamarin.Forms.SelectedPositionChangedEventArgs e)
        {
            if(this.Parent.Parent.Parent.Parent != null && this.Parent.Parent.Parent.Parent.GetType() == typeof(App))
            {

                HomeTab.UnselectedIconColor = Color.Gray;
                GiftTab.UnselectedIconColor = Color.Gray;
                AccountTab.UnselectedIconColor = Color.Gray;
                SettingTab.UnselectedIconColor = Color.Gray;


                switch (e.SelectedPosition)
                {
                    case 0:
                        ((App)(this.Parent.Parent.Parent.Parent)).MainPage = new NavigationPage(new Home());
                        HomeTab.UnselectedIconColor = Color.Black;
                        break;

                    case 1:
                        ((App)(this.Parent.Parent.Parent.Parent)).MainPage = new NavigationPage(new Desideri());
                        GiftTab.UnselectedIconColor = Color.Black;
                        break;

                    case 2:
                        ((App)(this.Parent.Parent.Parent.Parent)).MainPage = new NavigationPage(new EventiPersonali());
                        AccountTab.UnselectedIconColor = Color.Black;
                        break;

                    case 3:
                        ((App)(this.Parent.Parent.Parent.Parent)).MainPage = new NavigationPage(new Impostazioni());
                        SettingTab.UnselectedIconColor = Color.Black;
                        break;
                }
            }
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AddPage()));

        }
    }
}
