using Android.Content;
using AppRegali.Render;
using CustomRenderer.Android;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
namespace CustomRenderer.Android
{
    class MyEntryRenderer : MaterialEntryRenderer
    {
        public MyEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                //Control.HelperText.
                //Control.SetPadding(0, 0, 0, 0);
                //Control.EditText.SetPadding(0, 0, 0, 0);
            }

            if (e.NewElement != null)
            {
                //Control.SetPadding(0, 0, 0, 0);
                //Control.EditText.SetPadding(0, 0, 0, 0);
            }
        }
    }
}
