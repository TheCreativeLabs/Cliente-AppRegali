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
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage()
        {
            InitializeComponent();
        }

        private void btnTornaAllaHome_Clicked(object sender, EventArgs e)
        {
            //torno alla pagina di login.
            Application.Current.MainPage = new NavigationPage(new Login.Login());
        }
    }
}