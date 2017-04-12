using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Common.Android;
using Common.Android.Utilities;
using Common.Utilities.Authentication;

namespace PSA.Expense.Droid
{
    [Activity(Label = "My Expenses", Icon = "@drawable/Logo159x159", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
    Theme = "@style/AppTheme")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected ExpenseApp expenseApp;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);            

            global::Xamarin.Forms.Forms.Init(this, bundle);            
            expenseApp = new ExpenseApp(new AndroidAppUtilities(this));
            LoadApplication(expenseApp);
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

