using Common.Model;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    public delegate Task<bool> SaveExpenseHandler();

    /// <summary>
    /// Following Decorator Design Pattern, this is the base decorator class.
    /// It Mantains a reference to the ExpenseView object
    /// </summary>
    public abstract class ExpenseViewBehavior : IExpenseView
    {
        /// <summary>
        /// The View being decorated.
        /// </summary>
        public IExpenseView ExpenseView {get; protected set;}

        public ExpenseViewBehavior(IExpenseView expenseView)
        {
            if (expenseView == null)
            {
                throw new ArgumentNullException("Class decorator can't have a null IExpenseView");
            }
            this.ExpenseView = expenseView;
        }

        /// <summary>
        /// Delegate to call based expense view create content, which will initialize
        /// section and the items source
        /// </summary>
        public virtual async System.Threading.Tasks.Task CreateContent()
        {
            await this.ExpenseView.CreateContent();
        }

        /// <summary>
        /// Delegate to add a section in the based expense view
        /// </summary>
        /// <param name="section"></param>
        public void AddPage(Page section)
        {
            this.ExpenseView.AddPage(section);
        }

        /// <summary>
        /// Remove page from the view
        /// </summary>
        /// <param name="page">page to be removed</param>
        public void RemovePage(Page page)
        {
            this.ExpenseView.RemovePage(page);
        }

        /// <summary>
        /// Delegate to return the expense from the base view
        /// </summary>
        /// <returns></returns>
        public msdyn_expense GetExpense()
        {
            return this.ExpenseView.GetExpense();
        }

        /// <summary>
        /// Delegate to save expense
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Save()
        {
            return await this.ExpenseView.Save();
        }

        public bool IsValidExpense()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delegate to call change category
        /// </summary>
        /// <param name="newExpenseCategory"></param>
        /// <param name="warningMessage"></param>
        /// <returns></returns>
        public virtual async Task<bool> SetCategory(msdyn_expensecategory newExpenseCategory, string warningMessage = null)
        {
            return await this.ExpenseView.SetCategory(newExpenseCategory, warningMessage);
        }
    }
}
