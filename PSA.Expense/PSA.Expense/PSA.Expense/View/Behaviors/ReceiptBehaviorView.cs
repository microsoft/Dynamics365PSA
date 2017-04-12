using PSA.Expense.ViewModel;

namespace PSA.Expense.View
{
    public class ReceiptBehaviorView : ExpenseViewBehavior
    {
        protected ReceiptViewModel ViewModel;
        protected ReceiptsCollectionView ReceiptsView;

        public ReceiptBehaviorView(IExpenseView baseExpenseView) :base(baseExpenseView)
        {
            this.ViewModel = new ReceiptViewModel(this.GetExpense());
        }

        /// <summary>
        /// Add a new Section to show all the attached receipts of the selected expense.
        /// </summary>
        public override async System.Threading.Tasks.Task CreateContent()
        {
            await base.CreateContent();
            if (ReceiptsView == null)
            {
                ReceiptsView = new ReceiptsCollectionView(this.GetExpense());
                ReceiptsView.SaveExpense = this.Save;
                await ReceiptsView.Initialize(this.ViewModel);
                this.AddPage(ReceiptsView);
            }
        }
    }
}
