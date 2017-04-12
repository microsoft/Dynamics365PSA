using Common.Model;
using Common.Utilities;
using Common.Utilities.DataAccess.ServiceAccess;
using Common.Utilities.Extensions;
using Common.Utilities.Metadata;
using Common.Utilities.Resources;
using Common.View;
using Common.View.CustomControl;
using Common.View.ValueConverter;
using Common.ViewModel;
using Common.ViewModel.Command;
using PSA.Time.ViewModel;
using System;
using Xamarin.Forms;

namespace PSA.Time.View
{
    public class TimeDetailsPage : BaseContentPage
    {
        protected TimeViewModel ViewModel;
        protected PickerEx typePicker;
        protected PickerEx projectPicker;
        protected PickerEx projectTaskPicker;
        protected EntryEx durationEntry;
        protected DatePickerEx transactionDatePicker;
        protected EditorEx internalDescriptionEditor;
        protected EditorEx externalDescriptionEditor;
        protected ToolbarItem createTimeCommand;
        protected ToolbarItem deleteTimeCommand;
        protected ToolbarItem saveTimeCommand;
        protected ToolbarItem submitTimeCommand;
        protected ToolbarItem recallTimeCommand;
        protected LabelIcon filterIcon;
        protected StackLayout projectInfoLayout;

        public TimeDetailsPage()
            : base()
        {
            // Since the back button is needed on iOS, show only the back button (no title).
            this.Title = Device.OS == TargetPlatform.iOS ? "" : AppResources.Details;

            // Disable the back button for Android and WinPhone, enable it only for iOS.
            NavigationPage.SetHasBackButton(this, Device.OS == TargetPlatform.iOS ? true : false);
        }

        /// <summary>
        /// Initialize Time view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void InitViewModel(BaseViewModel viewModel)
        {
            this.ViewModel = viewModel as TimeViewModel;
            if (this.ViewModel == null)
            {
                this.ViewModel = new TimeViewModel(new msdyn_timeentry());
            }
        }

        /// <summary>
        /// Load reference data and set defaults 
        /// </summary>
        /// <returns></returns>
        protected override async System.Threading.Tasks.Task LoadData()
        {
            await ViewModel.LoadReferenceData();
            ViewModel.SetDefaultValues();
        }

        protected override void OnBindingContextChanged()
        {
            typePicker.SetBinding(PickerEx.SelectedItemProperty, new Binding(path: "Time.msdyn_type", converter: new OptionSetConverter<msdyn_timeentrytype>(), mode: BindingMode.TwoWay));
            typePicker.ItemsSource = ViewModel.GetAllOptionSetValues<msdyn_timeentrytype>();

            projectPicker.SetBinding(PickerEx.SelectedItemProperty, new Binding(path: "Time.msdyn_project", converter: new EntityPreviewConverter<msdyn_project>(), converterParameter: this.ViewModel, mode: BindingMode.TwoWay));
            projectPicker.ItemsSource = ViewModel.GetReferenceKeys<msdyn_project>(msdyn_project.EntityLogicalName);

            projectTaskPicker.SetBinding(PickerEx.SelectedItemProperty, new Binding(path: "Time.msdyn_projectTask", converter: new EntityPreviewConverter<msdyn_projecttask>(), converterParameter: this.ViewModel, mode: BindingMode.TwoWay));

            this.setEditabilityOfControls(ViewModel.CanEdit());
            base.OnBindingContextChanged();
        }

        /// <summary>
        /// Sets editablilty of all the controls in this page.
        /// </summary>
        /// <param name="canEdit">True if the control can be edited, false otherwise.</param>
        private void setEditabilityOfControls(bool canEdit)
        {            
            typePicker.IsEnabled = canEdit;            
            projectPicker.IsEnabled = canEdit;
            projectTaskPicker.IsEnabled = canEdit;
            transactionDatePicker.IsEnabled = canEdit;
            durationEntry.IsEnabled = canEdit;
            internalDescriptionEditor.IsEnabled = canEdit;
            externalDescriptionEditor.IsEnabled = canEdit;

            if (filterIcon != null)
            {
                filterIcon.IsEnabled = canEdit;
            }
        }

        protected override void SetBindingContext()
        {
            typePicker = ControlFactory.CreatePicker();
            typePicker.SelectedIndexChanged += TypePicker_SelectedIndexChanged;
            typePicker.SelectedItemChanged += Picker_SelectedItemChanged;

            projectPicker = ControlFactory.CreatePicker();
            projectPicker.HorizontalOptions = LayoutOptions.FillAndExpand;
            projectPicker.SelectedItemChanged += Picker_SelectedItemChanged;

            // Create a filter for showing projects only if the time entry can be edited.
            if (this.ViewModel.CanEdit())
            {
                filterIcon = ControlFactory.CreateIcon(LabelHandler.FILTER_SYMBOL, ControlFactory.Small_Label_Icon);
            }

            projectTaskPicker = ControlFactory.CreatePicker();
            projectTaskPicker.HorizontalOptions = LayoutOptions.FillAndExpand;
            projectTaskPicker.SelectedItemChanged += Picker_SelectedItemChanged;

            transactionDatePicker = ControlFactory.CreateDatePicker("Time.msdyn_date_utc", DateTime.MaxValue, "D");

            // Use a one-way binding from view model to view because of the custom formatting that we have. We want the user to be able to type anything in the text box 
            // but on losing focus from the field, we validate and store the duration in the view model.
            durationEntry = ControlFactory.CreateEntry("Time.msdyn_duration", BindingMode.OneWay, Keyboard.Default, null, "", new DurationTo24HourStringConverter());
            durationEntry.Unfocused += DurationEntry_Unfocused;

            internalDescriptionEditor = ControlFactory.CreateEditor("Time.msdyn_description");
            internalDescriptionEditor.TextChanged += this.createEditorTextChangedHandler(msdyn_timeentry.DescriptionLength);
            externalDescriptionEditor = ControlFactory.CreateEditor("Time.msdyn_externalDescription");
            externalDescriptionEditor.TextChanged += this.createEditorTextChangedHandler(msdyn_timeentry.DescriptionLength);

            this.BindingContext = ViewModel;
        }

        private void DurationEntry_Unfocused(object sender, FocusEventArgs e)
        {
            EntryEx entryCell = sender as EntryEx;
            BindDurationToViewModel(entryCell);
        }

        /// <summary>
        /// Converts the integer duration to a string format for the entry cell.
        /// </summary>
        /// <param name="duration">The duration for which to convert.</param>
        /// <returns>Formatted string that represents the duration.</returns>
        private string ConvertDurationToStringFormat(int? duration)
        {
            DurationTo24HourStringConverter converter = new DurationTo24HourStringConverter();
            return (string)converter.Convert(duration, Type.GetType("System.String"), null, null);
        }

        /// <summary>
        /// Copies the duration value of the current time entry to the view model (manually binding the value).
        /// </summary>        
        /// <param name="entryCell">The duration entry cell object.</param>
        /// <returns>True if the duration can be binded, false otherwise.</returns>
        private bool BindDurationToViewModel(EntryEx entryCell)
        {
            bool result = false;

            if (entryCell != null)
            {
                string stringValue = entryCell.Text;

                if (stringValue.Length > 0)
                {
                    string[] stringDuration = stringValue.Split(':');
                    int? previousTime = this.ViewModel.Time.msdyn_duration;
                    int hours, minutes;

                    // Throw error for an invalid input format string.
                    // We allow numbers in the format HH:mm or HH.
                    if (stringDuration.Length == 2)
                    {
                        // Throw an error to user if a non-integer or a negative number is entered and reset to the last valid data. 
                        if (!int.TryParse(stringDuration[0], out hours) || !int.TryParse(stringDuration[1], out minutes))
                        {
                            MessageCenter.ShowDialog(AppResources.InvalidDuration, null, null).DoNotAwait();
                            entryCell.Text = ConvertDurationToStringFormat(previousTime);
                            result = false;
                        }
                        // Throw an error if the string format is not a 24-hour system duration and reset to the last valid data.
                        else if ((hours > 23 || minutes > 59) && !(hours == 24 && minutes == 0))
                        {
                            MessageCenter.ShowDialog(AppResources.InvalidDuration, null, null).DoNotAwait();
                            entryCell.Text = ConvertDurationToStringFormat(previousTime);
                            result = false;
                        }
                        else
                        {
                            this.ViewModel.Time.msdyn_duration = hours * 60 + minutes;
                            result = true;
                        }
                    }
                    else if (stringDuration.Length == 1)
                    {
                        // Throw an error to user if a non-integer or a negative number is entered and reset to the last valid data. 
                        if (!int.TryParse(stringDuration[0], out hours))
                        {
                            MessageCenter.ShowDialog(AppResources.InvalidDuration, null, null).DoNotAwait();
                            entryCell.Text = ConvertDurationToStringFormat(previousTime);
                            result = false;
                        }
                        // Throw an error if the string format is not a 24-hour system duration and reset to the last valid data.
                        else if (hours > 24)
                        {
                            MessageCenter.ShowDialog(AppResources.InvalidDuration, null, null).DoNotAwait();
                            entryCell.Text = ConvertDurationToStringFormat(previousTime);
                            result = false;
                        }
                        else
                        {
                            this.ViewModel.Time.msdyn_duration = hours * 60;
                            result = true;
                        }
                    }
                    // If the format of the entry is incorrect, throw an error message and reset to the last valid data.
                    else
                    {
                        MessageCenter.ShowDialog(AppResources.InvalidDuration, null, null).DoNotAwait();
                        entryCell.Text = ConvertDurationToStringFormat(previousTime);
                        result = false;
                    }
                }
            }
            return result;
        }

        private EventHandler<TextChangedEventArgs> createEditorTextChangedHandler(int allowedTextSize)
        {
            return (object sender, TextChangedEventArgs e) =>
            {
                EditorEx editor = sender as EditorEx;

                if (e.OldTextValue == null && this.ViewModel.HasPendingDataToSave)
                {
                    this.ViewModel.HasPendingDataToSave = false;
                }

                if (e.NewTextValue.Length > allowedTextSize)
                {
                    editor.Text = e.NewTextValue.Substring(0, allowedTextSize);

                    // Let user know you've truncated the text.
                    MessageCenter.ShowMessage(string.Format(AppResources.TextSizeLimitedTo, allowedTextSize)).DoNotAwait();
                }

                editor.InvalidateLayout();
            };
        }

        private void TypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            PickerEx picker = (PickerEx)sender;
            if (picker.SelectedItem != null && projectInfoLayout != null)
            {
                msdyn_timeentrytype type = ((msdyn_timeentrytype[])picker.ItemsSource)[picker.SelectedIndex];

                projectInfoLayout.IsVisible = (type == msdyn_timeentrytype.Work);

                if (type != msdyn_timeentrytype.Work)
                {
                    this.ViewModel.Time.msdyn_project = null;
                }
            }
        }

        private void Picker_SelectedItemChanged(object sender, object oldvalue, object newvalue)
        {
            if (oldvalue == null && this.ViewModel.HasPendingDataToSave)
            {
                this.ViewModel.HasPendingDataToSave = false;
            }
            if (sender == projectPicker && newvalue != null && newvalue.ToString().Length > 0)
            {
                UpdateProjectTasks();
            }
        }

        protected override void CreateContent(StackLayout details)
        {
            Boolean isFilterEnabled = filterIcon != null;

            // For performance reasons, only if the filter is needed do we add it to the layout.
            this.createProjectInfoLayout(isFilterEnabled);

            StackLayout baseStackLayout = new StackLayout
            {
                Padding = 10,
                Children =
                {
                    ControlFactory.CreateStaticLabel(AppResources.Date),
                    transactionDatePicker,
                    ControlFactory.CreateStaticLabel(AppResources.DurationText),
                    durationEntry,
                    ControlFactory.CreateStaticLabel(AppResources.Type),
                    typePicker,
                    projectInfoLayout,
                    ControlFactory.CreateStaticLabel(AppResources.InternalComments),
                    internalDescriptionEditor,
                    ControlFactory.CreateStaticLabel(AppResources.ExternalComments),
                    externalDescriptionEditor
                }
            };

            if (isFilterEnabled)
            {
                TapGestureRecognizer filterTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
                filterTapGestureRecognizer.Tapped += FilterProject;
                filterIcon.GestureRecognizers.Add(filterTapGestureRecognizer);
            }

            // Add Time preview at top of the page
            StackLayout TimePreview = new TimePreview();
            TimePreview.SetBinding<TimeViewModel>(Grid.BindingContextProperty, vm => vm.Time, BindingMode.OneWay);
            details.Children.Add(TimePreview);

            details.Children.Add(new ScrollView() { Content = baseStackLayout });
        }

        /// <summary>
        /// Create a project info layout with or without the filter. 
        /// </summary>
        /// <param name="filterEnabled">True if the filter is to be included, false otherwise.</param>
        private void createProjectInfoLayout(bool filterEnabled)
        {
            if (filterEnabled)
            {
                SetProjectFilterName(filterIcon).DoNotAwait();
            }

            // Create the stack layout object for project controls.
            projectInfoLayout = new StackLayout();
            projectInfoLayout.Children.Add(ControlFactory.CreateStaticLabel(AppResources.Project));
            StackLayout childStack = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal
            };

            // Add project picker and filter (if neccessary).
            childStack.Children.Add(projectPicker);
            if (filterEnabled)
            {
                StackLayout filterStack = new StackLayout()
                {
                    Padding = new Thickness(0, 6),
                    Children = {filterIcon}
                };
                childStack.Children.Add(filterStack);
            }

            projectInfoLayout.Children.Add(childStack);

            // Add project task picker.
            projectInfoLayout.Children.Add(ControlFactory.CreateStaticLabel(AppResources.ProjectTask));
            projectInfoLayout.Children.Add(new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    projectTaskPicker
                }
            });
        }

        /// <summary>
        /// Change current filter and open the look up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void FilterProject(object sender, EventArgs e)
        {
            if (filterIcon != null)
            {
                FilterQuery filter = await this.SetProjectFilterName(filterIcon, true);

                if (filter != null)
                {
                    // Set the project picker list and clear the project task picker when filter is clicked.
                    if (projectPicker != null)
                    {
                        projectPicker.ItemsSource = filter.Result;
                    }

                    if (projectTaskPicker != null)
                    {
                        projectTaskPicker.ItemsSource = null;
                    }
                }
            }
        }

        protected async void UpdateProjectTasks()
        {
            if (this.ViewModel.Time.msdyn_project != null)
            {
                projectTaskPicker.ItemsSource = await ViewModel.GetTasksForProject(this.ViewModel.Time.msdyn_project);
            }
        }

        /// <summary>
        /// Sets the label of the project filter
        /// </summary>
        /// <param name="filterIcon">Label with the filter symbol</param>
        /// <param name="switchFilter">True if before setting the name it should switch</param>
        /// <returns>Current Filter</returns>
        protected async System.Threading.Tasks.Task<FilterQuery> SetProjectFilterName(LabelIcon filterIcon, bool switchFilter = false)
        {
            if (filterIcon != null)
            {
                FilterQuery filter = await this.ViewModel.GetFilterProject(switchFilter);
                if (filter != null)
                {
                    filterIcon.Text = String.Format("{0} {1}", LabelHandler.FILTER_SYMBOL, filter.Name);
                    return filter;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if there is any data that needs to be saved before going back
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            if (ViewModel != null && ViewModel.CanEdit() && ViewModel.HasPendingDataToSave)
            {
                ShowUnsavedDataWarning();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        /// <summary>
        /// Shows a warning message to the user about unsaved data, if the user decides to discard the changes
        /// the app we will go back whiout saving data, otherwise will stay in the current form.
        /// </summary>
        protected async void ShowUnsavedDataWarning()
        {
            bool continueGoingBack = await MessageCenter.ShowDialog(AppResources.UnsavedDataWarning, null, AppResources.Discard);
            if (continueGoingBack)
            {
                ViewModel.HasPendingDataToSave = false;
                if (this.Navigation.NavigationStack.Count > 0)
                {
                    await this.Navigation.PopAsync();
                }
            }
        }

        protected override void SetToolbarItems()
        {
            base.SetToolbarItems();
            this.setEditableEntryToolbarItems();
            this.setSubmittedEntryToolbarItems();

            // Create new time entry from this page, applicable to all time entries.
            createTimeCommand = ToolbarHelper.createAddTimeEntryButton();
            createTimeCommand.Clicked += async (sender, args) =>
            {
                if (await this.saveTimeEntry(false))
                {
                    msdyn_timeentry timeEntry = new msdyn_timeentry();
                    TimeDetailsPage timeDetailsPage = new TimeDetailsPage();

                    this.Navigation.InsertPageBefore(timeDetailsPage, this);
                    await timeDetailsPage.Initialize(new TimeViewModel(timeEntry));
                    await this.Navigation.PopAsync();
                }
            };

            this.ToolbarItems.Add(createTimeCommand);
        }

        /// <summary>
        /// Set all the toolbar items required for editable time entries
        /// and removed other unwanted toolbar items.
        /// </summary>
        private void setEditableEntryToolbarItems()
        {
            if (ViewModel.CanEdit())
            {
                // Delete command.
                deleteTimeCommand = ToolbarHelper.createDeleteButton();
                deleteTimeCommand.Command = new RelayCommandAsync(async () =>
                {
                    // Since the toolbar items cannot be disabled, disable the action if
                    // there is already an action going on in the view model.
                    if (!this.ViewModel.IsBusy)
                    {
                        ViewModel.IsBusy = true;
                        this.setEditabilityOfControls(false);           // Prevent user from being able to edit while submit happens.

                        // Delete the time entry and go back to the caller page, if exists.
                        if (await this.ViewModel.Delete())
                        {
                            MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                            if (Navigation.NavigationStack.Count > 0)
                            {
                                await Navigation.PopAsync();
                            }
                        }
                        else
                        {   
                            // Delete failed so stay on the page and enable controls again.
                            this.setEditabilityOfControls(true);
                        }
                        ViewModel.IsBusy = false;
                    }
                });

                // Save command.
                saveTimeCommand = ToolbarHelper.createSaveButton();
                saveTimeCommand.Command = new RelayCommandAsync(async () =>
                {
                    await this.saveTimeEntry(true);
                });

                // Submit command.
                submitTimeCommand = ToolbarHelper.createSubmitButton();
                submitTimeCommand.Command = new RelayCommandAsync(async () =>
                {
                    // Since the toolbar items cannot be disabled, disable the action if
                    // there is already an action going on in the view model.
                    if (!this.ViewModel.IsBusy)
                    {
                        this.ViewModel.IsBusy = true;

                        // Required because the unfocused method (binding) is triggered after submit is completed.
                        // Only if the duration is entered in the correct format do we save.
                        if (BindDurationToViewModel(this.durationEntry))
                        {   
                            this.setEditabilityOfControls(false);           // Prevent user from being able to edit while submit happens.

                            // Submit the time entry and go back to the caller page.
                            if (await this.ViewModel.Submit() && this.Navigation.NavigationStack.Count > 0)
                            {
                                MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                                await this.Navigation.PopAsync();
                            }
                            else
                            {
                                // Submit failed so stay on the page and enable controls again.
                                await MessageCenter.ShowErrorMessage(AppResources.SubmitError);
                                this.setEditabilityOfControls(true);
                            }
                        }

                        // No need to reset editability of controls here since after submit this page is closed.
                        this.ViewModel.IsBusy = false;
                    }
                });

                // Clear the toolbar first to prevent double adding.
                this.ToolbarItems.Clear();
                this.ToolbarItems.Add(submitTimeCommand);
                this.ToolbarItems.Add(saveTimeCommand);
                this.ToolbarItems.Add(deleteTimeCommand);
            }
        }

        /// <summary>
        /// Set all the toolbar items required for a submitted time entry.
        /// </summary>
        private void setSubmittedEntryToolbarItems()
        {
            if (ViewModel.IsSubmitted())
            {
                // Recall command.
                recallTimeCommand = ToolbarHelper.createRecallButton();
                recallTimeCommand.Command = new RelayCommandAsync(async () =>
                {
                    // Since the toolbar items cannot be disabled, disable the action if
                    // there is already an action going on in the view model.
                    if (!this.ViewModel.IsBusy)
                    {
                        // No need to disable controls here since this control exists only for non-editable entries.
                        this.ViewModel.IsBusy = true;
                        // If recall finishes correctly, stay on the page since the user probably wants to edit the record.
                        if (await this.ViewModel.Recall())
                        {
                            this.setEditabilityOfControls(ViewModel.CanEdit());
                            this.setEditableEntryToolbarItems();
                            ViewModel.HasPendingDataToSave = false; // This needs to be manually changed here because the status has changed.                  
                        }
                        else
                        {
                            await MessageCenter.ShowErrorMessage(AppResources.RecallError);
                        }
                        this.ViewModel.IsBusy = false;
                    }
                });

                this.ToolbarItems.Add(recallTimeCommand);
            }
        }

        /// <summary>
        /// Open a new instance of TimeDetailPage.
        /// </summary>
        /// <param name="timeEntry">Time entry to show on the detail page.</param>
        /// <param name="navigation">Navigation to push the page on.</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task NavigateToTimeDetailsView(msdyn_timeentry timeEntry, INavigation navigation)
        {
            TimeDetailsPage timeDetailPage = new TimeDetailsPage();
            await navigation.PushAsync(timeDetailPage);
            await timeDetailPage.Initialize(new TimeViewModel(timeEntry));
        }

        /// <summary>
        /// Save the current time entry. 
        /// </summary>
        /// <param name="closePage">True if this method should close the page after saving, false otherwise.</param>
        /// <returns>Task<bool> to indicate success/failure of save.</bool></returns>
        private async System.Threading.Tasks.Task<bool> saveTimeEntry(bool closePage)
        {
            bool success = false;

            // Since the toolbar items cannot be disabled, disable the action if
            // there is already an action going on in the view model.
            if (!this.ViewModel.IsBusy)
            {
                ViewModel.IsBusy = true;

                // Required because the unfocused method (binding) is triggered after save is completed.
                // Save only if the duration is entered in the right format.
                if (BindDurationToViewModel(this.durationEntry))
                {
                    this.setEditabilityOfControls(false);           // Prevent user from being able to edit while submit happens.   

                    // Save the time entry and go back to the caller page, if exists.
                    if (await this.ViewModel.ForcedSave() && this.Navigation.NavigationStack.Count > 0)
                    {
                        MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                        success = true;

                        if (closePage)
                        {
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        // Save failed so stay on the page and enable controls again.
                        this.setEditabilityOfControls(true);
                    }
                }

                // No need to reset editability of controls here since after submit this page is closed.
                ViewModel.IsBusy = false;
            }
            return success;
        }    
    }
}
