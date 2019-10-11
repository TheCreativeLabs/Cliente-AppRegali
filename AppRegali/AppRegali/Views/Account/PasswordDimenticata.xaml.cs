using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppRegali.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordDimenticata : ContentPage
    {
        public PasswordDimenticata()
        {
            InitializeComponent();
        }

        private async void btnProsegui_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CambiaPassword());
        }
    }
}