using Common.Model;
using Common.View;
using Common.View.CustomControl;
using Common.View.ValueConverter;
using PSA.Time.View.ValueConverter;
using PSA.Time.ViewModel;
using Xamarin.Forms;

namespace PSA.Time.View
{
    public class TimePreview : StackLayout
    {
        public static int HeightTimePreview = Device.OnPlatform<int>(55, 55, 60);
        protected LabelIcon entryTypeIcon;
        protected Label projectNameLabel;        
        protected Label timeLabel;
        protected Label statusLabel;
        private LabelIcon selectedIcon;

        public TimePreview(TimeCollectionViewModel timeCollectionViewModel = null)
        {
            Orientation = StackOrientation.Vertical;
            Padding = 0;
            Spacing = 0;
            this.BackgroundColor = BaseApp.PAGE_BACKGROUND_COLOR;

            StackLayout rootLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 10,
                Padding = 6,
                Orientation = StackOrientation.Horizontal,
                
            };
            this.Children.Add(rootLayout);

            if (timeCollectionViewModel != null)
            {
                selectedIcon = ControlFactory.CreateIcon(null, ControlFactory.Medium_Label_Icon);
                selectedIcon.TextColor = Color.Black;
                selectedIcon.FontSize = Device.OnPlatform(10, 12, 16);
                selectedIcon.VerticalOptions = LayoutOptions.CenterAndExpand;
                selectedIcon.HorizontalOptions = LayoutOptions.Start;
                selectedIcon.SetBinding(LabelIcon.IsVisibleProperty, new Binding()
                {
                    Source = timeCollectionViewModel,
                    Path = "MultiselectModeEnabled"
                });
                selectedIcon.SetBinding(LabelIcon.TextProperty, "Selected", converter: new BooleanToSelectedIconConverter());
             
                rootLayout.Children.Add(selectedIcon);
            }

            entryTypeIcon = ControlFactory.CreateIcon(null, ControlFactory.Medium_Label_Icon);
            entryTypeIcon.SetBinding(Label.TextProperty, new Binding("msdyn_type", BindingMode.OneWay, new TimeEntryTypeToIconConverter()));
            entryTypeIcon.SetBinding(Label.TextColorProperty, new Binding(".", BindingMode.OneWay, new TimeEntryToColorConverter()));
            entryTypeIcon.VerticalOptions = LayoutOptions.Center;

            projectNameLabel = ControlFactory.CreateLabel(".", valueConverter: new TimeEntryToTitleConverter());
            projectNameLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
            projectNameLabel.VerticalOptions = LayoutOptions.FillAndExpand;

            timeLabel = ControlFactory.CreateLabel("msdyn_duration", "{0}", null, new DurationToStringConverter());
            timeLabel.XAlign = TextAlignment.End;            
            timeLabel.MinimumWidthRequest = 100;

            statusLabel = ControlFactory.CreateLabel("msdyn_entryStatus", "{0}", ControlFactory.Label_Indicator, new OptionSetConverter<msdyn_timeentry_msdyn_entrystatus>());
            statusLabel.XAlign = TextAlignment.End;
            statusLabel.VerticalOptions = LayoutOptions.End;

            StackLayout timeAndStatusLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.End,
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Spacing = 0,
                Children =
                {
                    timeLabel,
                    statusLabel
                }
            };

            rootLayout.Children.Add(entryTypeIcon);
            rootLayout.Children.Add(projectNameLabel);
            rootLayout.Children.Add(timeAndStatusLayout);
            rootLayout.Children.Add(new Grid
            {
                Padding = 0,
                WidthRequest = Device.OnPlatform<double>(15, 0, 0),
                BackgroundColor = Color.Transparent
            });

            this.Children.Add(new Grid
            {
                Padding = 0,
                HeightRequest = 1,
                BackgroundColor = Color.FromHex("dddddd")
            });
        }        
    }
}
