using Common.Utilities;
using PSA.Expense.View;
using Xamarin.Forms;

namespace PSA.Expense
{
    public class ExpenseApp : Common.View.BaseApp
    {
        public ExpenseApp(AppUtilities appComponents) : base(appComponents) { }

        public override Page GetMainPage()
        {
            return new ExpenseCollectionView();
        }

        /// <summary>
        /// The app client ID.
        /// </summary>
        protected override string AuthenticationClientId
        {
            get
            {
                return "98ee7f5e-34bc-4114-aa3b-d5bb80f5e761";
            }
        }
    }
}
