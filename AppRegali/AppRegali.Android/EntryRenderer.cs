using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Widget;
using AppRegali;
using AppRegali.Render;
using CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(MyEntryRenderer2))]
[assembly: ExportRenderer(typeof(SearchEntry2), typeof(SearchEntryRenderer2))]
[assembly: ExportRenderer(typeof(ChatEntry), typeof(ChatEditorRenderer))]
[assembly: ExportRenderer(typeof(SearchBar), typeof(SearchCustomRenderer))]

namespace CustomRenderer
{
    class MyEntryRenderer2 : EntryRenderer
    {
        public MyEntryRenderer2(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;

                RoundRectShape i = new RoundRectShape(
                         new float[] { 15, 15, 15, 15, 15, 15, 15, 15 },
                         null,
                         new float[] { 15, 15, 15, 15, 15, 15, 15, 15 });

                var shape = new ShapeDrawable(i);
                shape.Paint.Color = Xamarin.Forms.Color.FromRgb(184, 184, 184).ToAndroid();
                shape.Paint.SetStyle(Paint.Style.Stroke);
                nativeEditText.Background = shape;
                nativeEditText.SetPadding(25, 25, 25, 25);
            }
        }
    }



    class SearchEntryRenderer2 : EntryRenderer
    {
        public SearchEntryRenderer2(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;

                RoundRectShape i = new RoundRectShape(
                         new float[] { 15, 15, 15, 15, 15, 15, 15, 15 },
                         null,
                         new float[] { 15, 15, 15, 15, 15, 15, 15, 15 });

                var shape = new ShapeDrawable(i);
                shape.Paint.Color = Xamarin.Forms.Color.White.ToAndroid();
                shape.Paint.SetStyle(Paint.Style.Stroke);
                nativeEditText.Background = shape;
                nativeEditText.SetPadding(25, 25, 25, 25);
            }
        }


    }


    class ChatEditorRenderer : EditorRenderer
    {
        public ChatEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;

                RoundRectShape i = new RoundRectShape(
                         new float[] { 15, 15, 15, 15, 15, 15, 15, 15 },
                         null,
                         new float[] { 15, 15, 15, 15, 15, 15, 15, 15 });

                var shape = new ShapeDrawable(i);
                shape.Paint.Color = Xamarin.Forms.Color.White.ToAndroid();
                shape.Paint.SetStyle(Paint.Style.Stroke);
                nativeEditText.Background = shape;
                nativeEditText.SetPadding(15, 15, 15, 15);
            }
        }
    }


    class SearchCustomRenderer : SearchBarRenderer
    {
        public SearchCustomRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var nativeEditText = (global::Android.Widget.SearchView)Control;

                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                plate.SetBackgroundColor(Android.Graphics.Color.Transparent);

                RoundRectShape i = new RoundRectShape(
                         new float[] { 30, 30, 30, 30, 30, 30, 30, 30 },
                         null,
                         new float[] { 30, 30, 30, 30, 30, 30, 30, 30 });
                

                var shape = new ShapeDrawable(i);
                shape.Paint.Color = Xamarin.Forms.Color.Transparent.ToAndroid();
                shape.Paint.SetStyle(Paint.Style.Stroke);
                nativeEditText.Background = shape;
            }
        }
    }

}
