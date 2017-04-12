using Common.View;
using System.Windows.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Common.View.CustomControl.EditorEx), typeof(Common.WinPhone.Renderer.EditorExRenderer))]

namespace Common.WinPhone.Renderer
{
    /// <summary>
    /// Customer Renderer class
    /// </summary>
    public class EditorExRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {   
                var native = (TextBox)Control;
                
                // Set Border and AcceptReturn
                native.Margin = new System.Windows.Thickness(1, 1, 8, 5);
                native.Padding = new System.Windows.Thickness(5, 2, 5, 5);
                native.FontSize = 20;
                native.BorderThickness = new System.Windows.Thickness(1);
                native.MinHeight = 150;
                native.AcceptsReturn = true;
                native.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                native.IsEnabledChanged += native_IsEnabledChanged;
                
                native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR);
                native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR);
            }
        }

        /// <summary>
        /// Override colors of a disabled TextBox 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void native_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            TextBox native = sender as TextBox;
            if (native != null && !native.IsEnabled)
            {
                native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR_DISABLED);
                native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR_DISABLED);
            }
        }
    }
}