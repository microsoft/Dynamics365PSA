using Common.Model;
using Common.Utilities;
using Common.Utilities.Camera;
using Common.Utilities.DataAccess;
using Common.Utilities.Resources;
using Common.View;
using Common.ViewModel;
using PSA.Expense.ViewModel;
using System;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    public class ReceiptsCollectionView : BaseContentPage
    {
        protected ReceiptViewModel ViewModel;
        public SaveExpenseHandler SaveExpense;

        public ReceiptsCollectionView(msdyn_expense expense) : base()
        {
            this.Title = AppResources.Receipts;
        }

        protected override void InitViewModel(BaseViewModel viewModel)
        {
            this.ViewModel = viewModel as ReceiptViewModel;
        }

        protected override async System.Threading.Tasks.Task LoadData()
        {
            await ViewModel.Initialize();
        }

        protected override void SetBindingContext()
        {
            this.BindingContext = ViewModel;
        }

        protected override void SetToolbarItems()
        {
            base.SetToolbarItems();
            if (ViewModel.SelectedExpense.CanEdit())
            {
                // Add From Camera
                ToolbarItem captureReceipt = new ToolbarItem
                {
                    Text = AppResources.CaptureReceipt,
                    Icon = Device.OnPlatform("camera.png", "camera.png", "Assets/Icons/camera.png"),
                    Order = ToolbarItemOrder.Primary
                };

                captureReceipt.Clicked += (sender, args) =>
                {
                    CameraUtil.Current.OnCaptureCompleted += Camera_OnCaptureCompleted;
                    CameraUtil.Current.OpenCamera();
                };

                this.ToolbarItems.Add(captureReceipt);
            }
        }

        protected override void CreateContent(StackLayout details)
        {
            ListView listOfReceipts = new ListView
            {
                ItemsSource = ViewModel.AttachedNotes,
                ItemTemplate = new DataTemplate(CreateReceiptPreviewTemplate),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowHeight = ReceiptPreview.HeightReceiptPreview,
                SeparatorVisibility = SeparatorVisibility.None // Windows doesn't have visibility so setting to None and adding one manually 
            };

            listOfReceipts.ItemSelected += listOfReceipts_ItemSelected;

            details.Children.Add(listOfReceipts);
        }

        private ReceiptPreview CreateReceiptPreviewTemplate()
        {
            ReceiptPreview receiptPreview = new ReceiptPreview(this.ViewModel);
            return receiptPreview;
        }

        private async void listOfReceipts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Annotation note = e.SelectedItem as Annotation;
            if (note != null)
            {
                ViewModel.IsBusy = true;

                if (note.DocumentBody == null)
                {   // Fetch the body only if is not already in memory
                    note = await ViewModel.GetAdditionalInformation(note);
                }

                try
                {
                    string filePath = await DeviceDataAccess.Current.WriteBytesToPath(note.DocumentBody, note.FileName);

                    // Since there is no way to open the photo in iOS Photos app, show the image in a new page.
                    if (Device.OS != TargetPlatform.iOS)
                    {
                        await AppLauncher.Current.OpenFileAsync(filePath);
                    }
                    else
                    {                                            
                        ReceiptViewPage receiptPage = new ReceiptViewPage(filePath, note.FileName);
                        await this.Navigation.PushAsync(receiptPage);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert(AppResources.errorTitle, String.Format(AppResources.ErrorOpeningFile, ex.Message), AppResources.Cancel);
                }

                ViewModel.IsBusy = false;
            }

            // clear selection
            ListView listOfReceipts = sender as ListView;
            if (listOfReceipts != null)
            {
                listOfReceipts.SelectedItem = null;
            }
        }

        /// <summary>
        /// After a capture using the camera finished call view model to create the new receipt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private async void Camera_OnCaptureCompleted(object sender, EventArgs eventArgs)
        {
            this.ViewModel.IsBusy = true;

            try
            {
                if (SaveExpense == null || await SaveExpense())
                {
                    await ViewModel.AddReceipt(CameraUtil.Current.GetImageBytes());
                }
            }
            finally
            {
                CameraUtil.Current.OnCaptureCompleted -= Camera_OnCaptureCompleted;
            }
            
            this.ViewModel.IsBusy = false;
        }
    }
}
