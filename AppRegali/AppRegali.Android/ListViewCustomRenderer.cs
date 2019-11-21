using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using AppRegali;
using CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

// this line directly ubleow usings, before namespace declaration
[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(ColoredRefreshListViewRenderer))]
namespace CustomRenderer
{
    public class ColoredRefreshListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            try
            {
                var x = Control.Parent as SwipeRefreshLayout;
                x?.SetColorSchemeColors(global::Android.Graphics.Color.ParseColor("#c786d3"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

    }


}
