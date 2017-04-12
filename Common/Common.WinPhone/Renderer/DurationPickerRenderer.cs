using Common.View;
using System;
using System.Windows.Markup;
using System.Windows.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Common.View.CustomControl.DurationPickerEx), typeof(Common.WinPhone.Renderer.DurationPickerRenderer))]
namespace Common.WinPhone.Renderer
{
    /// <summary>
    /// Customer Renderer class
    /// </summary>
    public class DurationPickerRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var native = (Microsoft.Phone.Controls.TimePicker)Control;

                // Set TextColor and Border                
                native.FontSize = 20;
                native.HeaderTemplate = GetStyledHeaderTemplate(native.Header as string);
                native.Margin = new System.Windows.Thickness(1);
                native.Padding = new System.Windows.Thickness(0);
                native.BorderThickness = new System.Windows.Thickness(1);

                //Override default colors of Windows theme for an active date picker
                native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR);
                native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR);
                native.IsEnabledChanged += native_IsEnabledChanged;
            }
        }

        protected System.Windows.DataTemplate GetStyledHeaderTemplate(string text)
        {
            if (text != null)
            {
                string dataTemplateXaml =
                    String.Format(@"<DataTemplate
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <TextBlock 
                    Text=""{0}""
                    FontSize=""20""
                    Foreground=""Black"" />
            </DataTemplate>", text);

                return (System.Windows.DataTemplate)XamlReader.Load(dataTemplateXaml);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Override colors of a disabled date picker 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void native_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Microsoft.Phone.Controls.DatePicker native = sender as Microsoft.Phone.Controls.DatePicker;
            if (native != null && !native.IsEnabled)
            {
                native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR_DISABLED);
                native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR_DISABLED);
            }
        }
    }
}
