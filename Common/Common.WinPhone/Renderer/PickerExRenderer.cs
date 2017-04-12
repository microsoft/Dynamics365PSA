using Common.View;
using Microsoft.Phone.Controls;
using System;
using System.Windows.Markup;
using Xamarin.Forms.Platform.WinPhone;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Common.View.CustomControl.PickerEx), typeof(Common.WinPhone.Renderer.PickerExRenderer))]
namespace Common.WinPhone.Renderer
{
    /// <summary>
    /// Customer Renderer class
    /// </summary>
    public class PickerExRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var native = (ListPicker)((System.Windows.Controls.Grid)Control).Children[0];
                     
                native.HeaderTemplate = GetStyledHeaderTemplate(native.FullModeHeader as string);
                native.FontSize = 20;
                native.Margin = new System.Windows.Thickness(12, 2, 3, 8);
                native.Padding = new System.Windows.Thickness(0);
                native.BorderThickness = new System.Windows.Thickness(1);
                native.IsEnabledChanged += native_IsEnabledChanged;
                
                //Override default colors of Windows theme for an active picker
                native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR);
                native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR);
            }
        }

        /// <summary>
        /// Override colors of a disabled picker 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void native_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            ListPicker native = sender as ListPicker;
            if (native != null && !native.IsEnabled)
            {
                native.Foreground = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.TEXT_COLOR_DISABLED);
                native.BorderBrush = RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.CONTROL_BORDER_COLOR_DISABLED);
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
    }
}
