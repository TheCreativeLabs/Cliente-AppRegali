using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Api;
using Xamarin.Forms;

namespace AppRegali.Converter
{
    public class ProgressColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = (Color)App.Current.Resources["PrimaryColor"];
            if (value != null)
            {
                RegaloDtoOutput regalo = (RegaloDtoOutput)value;
                if (regalo.ImportoCollezionato.HasValue)
                {
                    var importo = regalo.ImportoCollezionato.Value / regalo.Prezzo;
                    if (importo == 1)
                    {
                        color = (Color)App.Current.Resources["SuccessColor"];
                    }
                }
            }
            return color;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }

}
