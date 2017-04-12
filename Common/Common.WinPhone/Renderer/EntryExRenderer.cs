using Common.View;
using Microsoft.Phone.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Common.View.CustomControl.EntryEx), typeof(Common.WinPhone.Renderer.EntryExRenderer))]
namespace Common.WinPhone.Renderer
{
    public class EntryExRenderer : EntryRenderer
    {
        /// <summary>
        /// Custom Renderer class
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                PhoneTextBox native = Control;
                Control.Margin = new System.Windows.Thickness(0, -3, 0, -3);
                if (native != null)
                {
                    // Set border, border color and put cursor at the beginning.
                    native.Margin = new System.Windows.Thickness(2, 1, 2, 2);
                    native.Padding = new System.Windows.Thickness(6, 5, 6, 10);
                    native.FontSize = 20;
                    native.BorderThickness = new System.Windows.Thickness(1);

                    //Set colors
                    native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR);
                    native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR);
                }
            }
        }
    }
}
