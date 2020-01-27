using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppRegali
{
    public class MyEntry : Entry
    {
    }

    public class SearchEntry : Entry
    {
    }

    public class SearchEntry2 : Entry
    {
    }

    public class DatePickerCtrl : DatePicker
    {
        public static readonly BindableProperty EnterTextProperty = BindableProperty.Create(propertyName: "Placeholder", returnType: typeof(string), declaringType: typeof(DatePickerCtrl), defaultValue: default(string));
        public string Placeholder
        {
            get;
            set;
        }
    }

    public class DatePickerBorderCtrl : DatePicker
    {
        public static readonly BindableProperty EnterTextProperty = BindableProperty.Create(propertyName: "Placeholder", returnType: typeof(string), declaringType: typeof(DatePickerBorderCtrl), defaultValue: default(string));
        public string Placeholder
        {
            get;
            set;
        }
    }

    public class ChatEntry : Editor
    {
    }
}
