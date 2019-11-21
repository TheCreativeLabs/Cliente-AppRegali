using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppRegali;
using CustomRenderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

// this line directly ubleow usings, before namespace declaration
[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(ColoredRefreshListViewRenderer))]
namespace CustomRenderer
{
    public class ColoredRefreshListViewRenderer : ListViewRenderer
    {
        ///<summary>
        /// Set the loader color to your custom color
        /// </summary>
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            var vc = ((UITableViewController)ViewController);

            // Set the color!
            if (vc?.RefreshControl != null)
                vc.RefreshControl.TintColor = UIColor.FromRGB(135, 200, 21); //fix color
        }
    }

}
