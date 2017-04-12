using Android.Widget;
using Common.Android.Helpers;
using Common.View.CustomControl;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LabelIcon), typeof(Common.Android.Renderer.LabelIconRenderer))]
namespace Common.Android.Renderer
{
    public class LabelIconRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            TextView native = Control as TextView;
            LabelIcon newElement = e.NewElement as LabelIcon;

            if (native != null && newElement != null)
            {
                var fontFamily = string.IsNullOrEmpty(newElement.FontFamily) ? "Fonts/DynamicsSymbol.ttf" : e.NewElement.FontFamily.TrimStart('/').Replace("Assets/", string.Empty);
                native.Typeface = TypefaceHelper.GetTypeface(Forms.Context, fontFamily);
            }
        }
    }
}
