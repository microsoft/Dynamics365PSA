using Common.View.CustomControl;
using System.Windows.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Common.View.CustomControl.LabelEx), typeof(Common.WinPhone.Renderer.LabelExRenderer))]
namespace Common.WinPhone.Renderer
{
    public class LabelExRenderer : LabelRenderer
    {
        /// <summary>
        /// Custom Renderer class
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                TextBlock native = Control as TextBlock;
                if (native != null)
                {
                    System.Windows.Thickness nativeThickness;
                    LabelEx newElement = e.NewElement as LabelEx;
                    if (newElement != null && newElement.Margin != null)
                    {
                        Thickness elementMargin = newElement.Margin.GetValueOrDefault();
                        nativeThickness = new System.Windows.Thickness(elementMargin.Left, elementMargin.Top, elementMargin.Right, elementMargin.Bottom);
                    }
                    else
                    {
                        nativeThickness = new System.Windows.Thickness(13, 0, 3, 0);
                    }
                    native.Margin = nativeThickness;
                }
            }
        }
    }
}
