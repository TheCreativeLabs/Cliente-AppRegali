using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppRegali.Utility
{
    //public class sealed CurrentCulture
    //{
    //}


    public sealed class CurrentCulture
    {
        private static CurrentCulture instance = null;
        //private static readonly object padlock = new object();
        public static CultureInfo Ci { get; set; } = new CultureInfo("en");

        CurrentCulture()
        {
            Ci = new CultureInfo("en");
        }

        public static CurrentCulture Instance
        {
            get
            {
                lock (Ci)
                {
                    if (instance == null)
                    {
                        instance = new CurrentCulture();
                    }
                    return instance;
                }
            }
        }

        public void SetCultureInfo(string language)
        {
            Ci = new CultureInfo(language);
        }
    }
}
