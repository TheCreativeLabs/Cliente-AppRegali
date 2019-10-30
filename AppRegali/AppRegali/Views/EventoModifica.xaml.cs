using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventoModifica : ContentPage
    {
        EventoDetailViewModel viewModel;

        public EventoModifica(EventoDetailViewModel eventoDetailViewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = eventoDetailViewModel;
        }

        protected override async void OnAppearing()
        {
            Stream stream = new MemoryStream(viewModel.Item.ImmagineEvento);
            imgEventoModifica.Source = ImageSource.FromStream(() => { return stream; });
        }
    }
}