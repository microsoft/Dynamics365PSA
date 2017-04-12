using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Common.Android;
using Common.Android.Utilities;
using Common.Utilities.Authentication;

namespace PSA.Time.Droid
{
    [Activity(Label = "My Time", Icon = "@drawable/ic_launcher", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, // UiOptions = UiOptions.SplitActionBarWhenNarrow,
    Theme = "@style/AppTheme")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected TimeApp TimeApp;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);            
            TimeApp = new TimeApp(new AndroidAppUtilities(this));
            LoadApplication(TimeApp);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            AndroidAppUtilities.SetContext(this);
        }

        /// <summary>
        /// This is callback method when user navigate back from ADAL Authentication
        /// </summary>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            // Pass the authentication result to ADAL.
            (Authentication.Current as AndroidAuthentication).ContinueAcquireToken(requestCode, resultCode, data);
        }
    }
}

