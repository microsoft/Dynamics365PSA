using Common.Utilities;
using Common.Utilities.Authentication;
using Common.Utilities.Metadata;
using Common.ViewModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.View
{
    public partial class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            InitializeComponent();
        }

        public async Task Initialize(BaseViewModel viewModel = null)
        {
            this.InitViewModel(viewModel);
            this.ToolbarItems.Clear();
            this.SetToolbarItems();
            await this.LoadData();
            this.SetBindingContext();
            this.CreateContent(this.details);
        }

        // Methods to override
        protected virtual void SetToolbarItems()
        {
        }

        protected virtual async Task LoadData()
        {
            await Task.Delay(0);
        }

        protected virtual void InitViewModel(BaseViewModel viewModel)
        {
        }

        protected virtual void SetBindingContext()
        {
        }

        public StackLayout GetDetailsLayout()
        {
            return this.details;
        }

        public ActivityIndicator GetActivityIndicator()
        {
            return this.activity;
        }

        /// <summary>
        /// Method that will initialize the details part of the page.
        /// </summary>
        /// <param name="details"></param>
        protected virtual void CreateContent(StackLayout details)
        {
            details.Children.Add(new Label { Text = "Welcome to PSA!" });
        }
    }
}
