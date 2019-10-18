using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppRegali.Render
{
   public class MyEntry: Entry
    {

        public static readonly BindableProperty PlaceholderFontFamilyProperty
            = BindableProperty.Create(nameof(PlaceholderFontFamily),
                                      typeof(string),
                                      typeof(MyEntry),
                                      default(string));

        public string PlaceholderFontFamily
        {
            get { return (string)GetValue(PlaceholderFontFamilyProperty); }
            set { SetValue(PlaceholderFontFamilyProperty, value); }
        }

        public MyEntry()
        {
        }
    }
}
