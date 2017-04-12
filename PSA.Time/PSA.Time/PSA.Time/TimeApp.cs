using Common.Utilities;
using PSA.Time.View;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PSA.Time
{
    public class TimeApp : Common.View.BaseApp
    {
        public TimeApp(AppUtilities appComponents)
            : base(appComponents)
        {
            
        }

        public override Page GetMainPage()
        {
            return new TimeCollectionView();
        }

        public override void SetColors()
        {
            TEXT_COLOR = Color.Black;
            TEXT_COLOR_DISABLED = Color.FromHex("FF404040");
            TEXT_COLOR_TITLE = Color.Black;

            // Need to contrast
            PAGE_BACKGROUND_COLOR = Color.White;
            PAGE_HEADER_COLOR = Color.FromHex("FF04A297");
            TEXT_COLOR_HEADER = PAGE_BACKGROUND_COLOR;
            TEXT_COLOR_LABEL = Color.Black;
            ICON_COLOR = Color.FromHex("FFC6C6C6");

            //Buttons:
            TEXT_COLOR_MAIN_ACTION = Color.FromHex("FF333333");

            //Borders:
            CONTROL_BORDER_COLOR = Color.Black;
            CONTROL_BORDER_COLOR_DISABLED = Color.White;
        }


        protected override string AuthenticationClientId
        {
            get
            {
                return "7f2902aa-5881-45af-bbc4-5b9796641600";
            }
        }

        protected override string AuthenticationRedirectUri
        {
            get
            {
                return "https://*.crm.dynamics.com";
            }
        }
    }
}
