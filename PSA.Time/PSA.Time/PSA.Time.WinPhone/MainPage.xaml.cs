using Common.WinPhone;
using Microsoft.Phone.Controls;

namespace PSA.Time.WinPhone
{
    partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        protected TimeApp app;

        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();
            app = new TimeApp(new WinPhoneAppUtilities());
            LoadApplication(app);
        }
    }
}
