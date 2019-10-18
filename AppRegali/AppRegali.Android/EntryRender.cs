using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using Xamarin.Forms.Material.Android;
using Android.Content;
using AppRegali.Render;
using CustomRenderer.Android;
using System.ComponentModel;
using Android.Text;
using Android.Text.Style;
using System.Text.RegularExpressions;

[assembly: ExportRenderer(typeof(MyEntry), typeof(CustomPlaceholderEntryRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
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
                Control.SetBackgroundColor(global::Android.Graphics.Color.LightGreen);
                Control.SetHintTextAppearance(AppRegali.Droid.Resource.Style.MyTextInputLayout);
                Control.SetHelperTextTextAppearance(AppRegali.Droid.Resource.Style.MyTextInputLayout);
                Control.EditText.SetHintTextColor(global::Android.Graphics.Color.Red);
            }
        }
    }

    public class CustomPlaceholderEntryRenderer : MaterialEntryRenderer
    {
        private MyEntry CustomElement => Element as MyEntry;

        public CustomPlaceholderEntryRenderer(Context context) : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            UpdatePlaceholderFont();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //if (e.PropertyName == MyEntry.PlaceholderFontFamilyProperty.PropertyName)
                UpdatePlaceholderFont();
        }

        private void UpdatePlaceholderFont()
        {
            if (CustomElement.PlaceholderFontFamily == default(string))
            {
                //Control.HintFormatted = null;
                Control.Hint = CustomElement.Placeholder;
                Control.TooltipText = "SADAD";
                var placeholderFontSize2 = (int)CustomElement.FontSize;
                var placeholderSpan2 = new SpannableString(CustomElement.Placeholder);
                placeholderSpan2.SetSpan(new AbsoluteSizeSpan(placeholderFontSize2, true), 0, placeholderSpan2.Length(), SpanTypes.InclusiveExclusive);
                Control.HintFormatted = placeholderSpan2;
                Control.SetHintTextAppearance(AppRegali.Droid.Resource.Style.MyTextInputLayout);

                //Control.DefaultHintTextColor = CustomElement.PlaceholderColor.ToAndroid();
                return;
            }

            var placeholderFontSize = (int)CustomElement.FontSize;
            var placeholderSpan = new SpannableString(CustomElement.Placeholder);
            placeholderSpan.SetSpan(new AbsoluteSizeSpan(placeholderFontSize, true), 0, placeholderSpan.Length(), SpanTypes.InclusiveExclusive); // Set Fontsize

            var typeFace = FindFont(CustomElement.PlaceholderFontFamily);
            var typeFaceSpan = new CustomTypefaceSpan(typeFace);
            placeholderSpan.SetSpan(typeFaceSpan, 0, placeholderSpan.Length(), SpanTypes.InclusiveExclusive); //Set Fontface

            Control.HintFormatted = placeholderSpan;
        }

        const string LoadFromAssetsRegex = @"\w+\.((ttf)|(otf))\#\w*";
        private Typeface FindFont(string fontFamily)
        {
            if (!string.IsNullOrWhiteSpace(fontFamily))
            {
                if (Regex.IsMatch(fontFamily, LoadFromAssetsRegex))
                {
                    var typeface = Typeface.CreateFromAsset(Context.Assets, FontNameToFontFile(fontFamily));
                    return typeface;
                }
                else
                {
                    return Typeface.Create(fontFamily, TypefaceStyle.Normal);
                }
            }

            return Typeface.Create(Typeface.Default, TypefaceStyle.Normal);
        }

        private string FontNameToFontFile(string fontFamily)
        {
            int hashtagIndex = fontFamily.IndexOf('#');
            if (hashtagIndex >= 0)
                return fontFamily.Substring(0, hashtagIndex);

            throw new InvalidOperationException($"Can't parse the {nameof(fontFamily)} {fontFamily}");
        }
    }

    public class CustomTypefaceSpan : TypefaceSpan
    {
        private readonly Typeface _customTypeface;

        public CustomTypefaceSpan(Typeface type) : base("")
        {
            _customTypeface = type;
        }

        public CustomTypefaceSpan(string family, Typeface type) : base(family)
        {
            _customTypeface = type;
        }

        public override void UpdateDrawState(TextPaint ds)
        {
            ApplyCustomTypeFace(ds, _customTypeface);
        }

        public override void UpdateMeasureState(TextPaint paint)
        {
            ApplyCustomTypeFace(paint, _customTypeface);
        }

        private static void ApplyCustomTypeFace(Paint paint, Typeface tf)
        {
            paint.SetTypeface(tf);
        }
    }
}

