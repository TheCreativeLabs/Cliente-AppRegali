using AppRegali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelezioneCategoria : ContentPage
    {

        CategorieViewModel viewModel;

        public SelezioneCategoria()
        {
            InitializeComponent();

            BindingContext = viewModel = new CategorieViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}