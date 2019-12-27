using System;
using Android.Webkit;
using AppRegali.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(ClearCookies))]
namespace AppRegali.Droid
{
    public class ClearCookies : IClearCookies
    {
        public void ClearAllCookies()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }
    }
}
