using Common.Model;

namespace PSA.Expense.View
{
    /// <summary>
    /// Class to get the instance of the expense view based on Setup
    /// </summary>
    public static class ExpenseViewFactory
    {
        /// <summary>
        /// Decorate the base expense View based on the type and behaviors of the current expense.
        /// </summary>
        /// <param name="expense"></param>
        public static ExpenseMainView CreateExpenseView(msdyn_expense expense)
        {
            // base view for any kind of expense
            ExpenseMainView expenseView = new ExpenseMainView(expense);
            IExpenseView decoratedExpenseView = expenseView;

            // Decorate with Receipt behavior
            decoratedExpenseView = new ReceiptBehaviorView(expenseView);

            expenseView.ExtendedExpenseBehavior = decoratedExpenseView;
            decoratedExpenseView.CreateContent();
            return expenseView;
        }

        /// <summary>
        /// Change at runtime the behaviors of the selected expense
        /// </summary>
        /// <param name="currentExpenseView"></param>
        /// <returns></returns>
        public static IExpenseView DecorateExpenseView(IExpenseView currentExpenseView)
        {
            if (currentExpenseView != null)
            {
                msdyn_expense expense = currentExpenseView.GetExpense();
                IExpenseView decoratedExpenseView = currentExpenseView;

                // Check if Receipt Decorator still valid
                ReceiptBehaviorView receiptBehavior = decoratedExpenseView as ReceiptBehaviorView;

                return decoratedExpenseView;
            }
            return null;
        }
    }
}
