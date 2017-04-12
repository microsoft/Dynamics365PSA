using Common.View.CustomControl;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LabelIcon), typeof(Common.iOS.Renderer.LabelIconRenderer))]
namespace Common.iOS.Renderer
{
    public class LabelIconRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            var native = (UILabel)Control;
            LabelIcon newElement = e.NewElement as LabelIcon;

            if (native != null && newElement != null)
            {
                var fontFamily = string.IsNullOrEmpty(newElement.FontFamily) ? "Dynamics Symbol" : e.NewElement.FontFamily;
                native.Font = UIFont.FromName(fontFamily, native.Font.PointSize);
            }
        }
    }
}