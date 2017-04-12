using Common.Model;
using Common.Utilities.Resources;
using Common.View;
using PSA.Time.ViewModel;
using System;
using Xamarin.Forms;
using Tasks = System.Threading.Tasks;

namespace PSA.Time.View
{
    public class TimeTabbedPage : ContentView
    {
        // Controls
        public TimePeriodSwitcher periodSwitcher;
        public ListView listView;
        public Label noItemsLabel;

        // ViewModel
        private TimeCollectionViewModel viewModel;

        public TimeTabbedPage(TimeCollectionViewModel viewModel) : base()
        {
            this.viewModel = viewModel;
            
            // Main layout
            StackLayout mainLayout = new StackLayout();
            mainLayout.Spacing = 0;
            mainLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            // Title only on windows phone. There is a title added for the other platforms.
            Device.OnPlatform(WinPhone: () =>
            {
                Label header = new Label();
                header.SetBinding(Label.TextProperty, "Title");
                header.BackgroundColor = TimeApp.PAGE_HEADER_COLOR;              
                header.Style = (Style)Application.Current.Resources["Label_PageHeader"];

                mainLayout.Children.Add(header);
            });            

            // Period switcher
            periodSwitcher = new TimePeriodSwitcher(this.viewModel);

            // List view
            this.initializeListView();

            // No items found label
            noItemsLabel = new Label
            {
                Text = AppResources.NoEntries,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = false,
                HeightRequest = 50
            };

            mainLayout.Children.Add(periodSwitcher);
            mainLayout.Children.Add(noItemsLabel);
            mainLayout.Children.Add(listView);

            this.Content = mainLayout;

            // We only update the visibility bindings once the first load completes
            this.viewModel.LoadTimesCompleted += this.ViewModel_LoadTimesCompleted;
        }

        private void initializeListView()
        {
            listView = new ListView
            {
                ItemTemplate = new DataTemplate(this.LoadTimePreviewTemplate),
                IsGroupingEnabled = true,
                // iOS list scrolls top item out of view when HasUnevenRows=true.
                HasUnevenRows = Device.OnPlatform(false, true, true),
                GroupShortNameBinding = new Binding(path: "Date", stringFormat: "{0:dd}"),
                GroupHeaderTemplate = new DataTemplate(typeof(TimeSectionHeaderView)),
                IsPullToRefreshEnabled = true,
                SeparatorVisibility = SeparatorVisibility.None,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            listView.ItemSelected += ListView_ItemSelectedNormal;
        }

        private async void ListView_ItemSelectedNormal(object sender, SelectedItemChangedEventArgs e)
        {
            // async void is OK here, since this is a top level event handler
            msdyn_timeentry selectedTime = e.SelectedItem as msdyn_timeentry;

            if (selectedTime != null)
            {
                await this.NavigateToTimeDetailsView(selectedTime);
                // Deselect the item
                listView.SelectedItem = null;
            }
        }

        private void ListView_ItemSelectedMultiSelect(object sender, SelectedItemChangedEventArgs e)
        {
            msdyn_timeentry selectedTime = e.SelectedItem as msdyn_timeentry;
            if (selectedTime != null)
            {
                // Switch selection status
                selectedTime.Selected = !selectedTime.Selected;

                listView.SelectedItem = null;
            }
        }

        protected async Tasks.Task NavigateToTimeDetailsView(msdyn_timeentry Time)
        {
            await TimeDetailsPage.NavigateToTimeDetailsView(Time, this.Navigation);
        }

        public ViewCell LoadTimePreviewTemplate()
        {
            // Walk parent views until we find a page
            VisualElement v = this.ParentView;
            while (!(v is Page))
            {
                v = v.ParentView;
            }
            Page hostPage = (Page)v;

            TimeCell timeCell = new TimeCell(hostPage);
            timeCell.View = new TimePreview(this.viewModel);
            return timeCell;
        }

        /// <summary>
        /// Enable or disable the multiselect mode.
        /// </summary>
        /// <param name="multiselectEnabled">bool indicating if multiselect should be enabled or disabled.</param>
        public void setMultiSelectMode(bool multiselectEnabled)
        {
            if (multiselectEnabled)
            {
                this.periodSwitcher.setSwitchingEnabled(false);

                listView.ItemSelected -= this.ListView_ItemSelectedNormal;
                listView.ItemSelected += this.ListView_ItemSelectedMultiSelect;
            }
            else
            {
                this.periodSwitcher.setSwitchingEnabled(true);

                listView.ItemSelected -= this.ListView_ItemSelectedMultiSelect;
                listView.ItemSelected += this.ListView_ItemSelectedNormal;

                // Unselect all
                this.viewModel.ClearSelection();
            }
        }

        private void setVisibilityBindings()
        {
            // Add the bindings for visibility now that a load is completed.
            noItemsLabel.SetBinding(Label.IsVisibleProperty, "HasItems", converter: new BoolInverseConverter());
            listView.SetBinding(ListView.IsVisibleProperty, "HasItems");
        }

        private void ViewModel_LoadTimesCompleted(object sender, EventArgs e)
        {
            this.viewModel.LoadTimesCompleted -= this.ViewModel_LoadTimesCompleted;
            this.setVisibilityBindings();
        }
    }
}
