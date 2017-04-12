using Xamarin.Forms;

namespace PSA.Time.View
{
    public class TimeSectionHeaderView : ViewCell
    {
        public TimeSectionHeaderView ()
        {
            this.Height = 40;

            var title = new Label
            {               
                FontAttributes = FontAttributes.Bold,                
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center
            };

            title.SetBinding(Label.TextProperty, new Binding(path: "Date", stringFormat: "{0:D}"));

            View = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 25,
                BackgroundColor = Color.FromHex("f5f5f5"),
                Padding = 5,
                Orientation = StackOrientation.Horizontal,
                Children = { title }
            };
        }
    }
}
