using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Api;
using Xamarin.Forms;

namespace AppRegali.Converter
{
    public class TabColorConverter : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            Color retSource = Color.Red;

            if (value != null) {

                var i = value;
            }  
            
            return retSource; 
        } 
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException(); 
        } 
    }
}
