﻿using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppRegali.Services;
using AppRegali.Views;
using AppRegali.Views.Login;
using Plugin.Multilingual;
using AppRegali.Api;

namespace AppRegali
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<Helpers.TranslateExtension>();
            // MainPage = new NavigationPage(new Login());
            SetMainPage();
        }

        public async void SetMainPage()
        {
            if (await ApiHelper.GetToken() != null)
            {
                MainPage = new NavigationPage(new Home());
            } else
            {
                MainPage = new NavigationPage(new Login());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
