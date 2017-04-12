using Common.WinPhone;
using Microsoft.Phone.Controls;

namespace PSA.Expense.WinPhone
{
    partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        protected ExpenseApp app;

        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();
            app = new ExpenseApp(new WinPhoneAppUtilities());
            LoadApplication(app);
        }
    }
}
