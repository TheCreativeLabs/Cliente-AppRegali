using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Api;
using Xamarin.Forms;

namespace AppRegali.Converter
{
    public class DonazioneProgress : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            double retSource = 0;

            if (value != null) {
                RegaloDtoOutput regalo = (RegaloDtoOutput)value;
                if (regalo.ImportoCollezionato.HasValue)
                    retSource = regalo.ImportoCollezionato.Value / regalo.Prezzo;
            }  
            
            return retSource; 
        } 
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException(); 
        } 
    }
}
