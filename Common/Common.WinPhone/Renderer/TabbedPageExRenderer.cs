using Common.View;
using System.Windows.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Common.View.CustomControl.TabbedPageEx), typeof(Common.WinPhone.Renderer.TabbedPageExRenderer))]
namespace Common.WinPhone.Renderer
{    
    public class TabbedPageExRenderer : TabbedPageRenderer
    {
        /// <summary>
        /// Custom Renderer class for Tabbed page in Windows
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            this.HeaderTemplate = GetStyledHeaderTemplate();
            this.TitleTemplate = GetStyledTitleTemplate();
            this.Template = GetTemplate();
        }

        private System.Windows.Controls.ControlTemplate GetTemplate()
        {
            string controlTemplateXaml = @"<ControlTemplate 
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            xmlns:phone=""clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone""
            xmlns:Primitives=""clr-namespace:Microsoft.Phone.Controls.Primitives;assembly=Microsoft.Phone""
            TargetType=""phone:Pivot"">
                        <Grid>
                            <Grid.RowDefinitions>
                                <RowDefinition Height=""Auto""/>
                                <RowDefinition Height=""Auto""/>
                                <RowDefinition Height=""*""/>
                            </Grid.RowDefinitions>
                            <Grid Background=""{TemplateBinding Background}"" Grid.RowSpan=""3""/>
                            <Grid Background=""" + RendererUtil.FromXamarinColorToWindowsBrush(BaseApp.PAGE_HEADER_COLOR).Color.ToString() + @""">
                                <ContentControl x:Name=""TitleElement"" ContentTemplate=""{TemplateBinding TitleTemplate}"" Content=""{TemplateBinding Title}"" Margin=""0,0,0,14""/>
                            </Grid>
                            <Primitives:PivotHeadersControl x:Name=""HeadersListElement"" Grid.Row=""1"" Margin=""0,10,0,5""/>
                            <ItemsPresenter x:Name=""PivotItemPresenter"" Grid.Row=""2""/>
                        </Grid>
                    </ControlTemplate>";
            return  XamlReader.Load(controlTemplateXaml) as System.Windows.Controls.ControlTemplate;
        }

        protected System.Windows.DataTemplate GetStyledHeaderTemplate()
        {
            string dataTemplateXaml =
                @"<DataTemplate
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <TextBlock 
                    Text=""{Binding Title}"" 
                    FontSize=""28""
                    Foreground=""#FF595959"" />
            </DataTemplate>";

            return (System.Windows.DataTemplate)XamlReader.Load(dataTemplateXaml);
        }

        protected System.Windows.DataTemplate GetStyledTitleTemplate()
        {
            string dataTemplateXaml =
                @"<DataTemplate
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <TextBlock 
                    Text=""{Binding}"" 
                    FontSize=""18""
                    Foreground=""White""
                    TextWrapping=""Wrap""/>
            </DataTemplate>";

            return (System.Windows.DataTemplate)XamlReader.Load(dataTemplateXaml);
        }
    }
}
