using Common.Utilities.Metadata;
using PSA.Time.ViewModel;
using System;
using Xamarin.Forms;

namespace PSA.Time.View
{
    /// <summary>
    /// Bar that has left and right arrow buttons, and display in the middle showing the current month.
    /// </summary>
    public class TimePeriodSwitcher : StackLayout
    {
        protected Button rightButton;
        protected Button leftButton;
        protected Label rangeLabel;

        private TimeCollectionViewModel viewModel;

        public TimePeriodSwitcher(TimeCollectionViewModel parentViewModel) : base()
        {
            viewModel = parentViewModel;

            Orientation = StackOrientation.Horizontal;
            BackgroundColor = Color.FromHex("f8f8f8");
            HeightRequest = Device.OnPlatform<int>(50, 50, 70);
            HorizontalOptions = LayoutOptions.FillAndExpand;

            rightButton = new Button
            {
                Text = LabelHandler.GREATERTHAN_SYMBOL,
                Font = Font.SystemFontOfSize(NamedSize.Medium),
                BorderWidth = 0,
                WidthRequest = 70,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            rightButton.Clicked += RightButtonClicked;

            leftButton = new Button
            {
                Text = LabelHandler.LESSTHAN_SYMBOL,
                Font = Font.SystemFontOfSize(NamedSize.Medium),
                BorderWidth = 0,
                WidthRequest = 70,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            leftButton.Clicked += LeftButtonClicked;          

            rangeLabel = new Label
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            rangeLabel.SetBinding(Label.TextProperty, new Binding("Filter.FilterText"));

            Children.Add(leftButton);
            Children.Add(rangeLabel);
            Children.Add(rightButton);
        }

        /// <summary>
        /// Moves the control to the next month.
        /// </summary>
        /// <param name="sender">The sender object for the event.</param>
        /// <param name="e">EventArgs for this event.</param>
        private async void RightButtonClicked(object sender, EventArgs e)
        {
            // Async void OK for top level event handler.
            await this.viewModel.IncrementDateFilter();
        }

        /// <summary>
        /// Moves the control to the previous month.
        /// </summary>
        /// <param name="sender">The sender object for the event.</param>
        /// <param name="e">EventArgs for this event.</param>
        private async void LeftButtonClicked(object sender, EventArgs e)
        {
            // Async void OK for top level event handler.
            await this.viewModel.DecrementDateFilter();
        }

        /// <summary>
        /// Allows enabling or disabling the buttons to switch months.
        /// </summary>
        /// <param name="switchAllowed">bool indicating if the control should allow switching periods.</param>
        public void setSwitchingEnabled(bool switchAllowed)
        {            
            this.rightButton.IsVisible = switchAllowed;
            this.leftButton.IsVisible = switchAllowed;
        }
    }
}
