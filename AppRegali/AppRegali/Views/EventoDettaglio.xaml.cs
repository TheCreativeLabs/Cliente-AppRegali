using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AppRegali.Models;
using AppRegali.ViewModels;
using Api;

namespace AppRegali.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EventoDettaglio : ContentPage
    {
        EventoDetailViewModel viewModel;

        public EventoDettaglio(EventoDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public EventoDettaglio()
        {
            InitializeComponent();

            var item = new EventoDtoOutput
            {
                Titolo = "Item 1",
                Descrizione = "This is an item description."
            };

            viewModel = new EventoDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}