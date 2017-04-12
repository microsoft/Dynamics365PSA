using Common.Model;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    /// <summary>
    /// Defines the common methods between expense behaviors.
    /// </summary>
    public interface IExpenseView
    {
        /// <summary>
        /// Draw UI elements based on the correct behaviors
        /// </summary>
        System.Threading.Tasks.Task CreateContent();

        /// <summary>
        /// Add a new page to the view
        /// </summary>
        /// <param name="page">new page to be added</param>
        void AddPage(Page page);

        /// <summary>
        /// Remove page from the view
        /// </summary>
        /// <param name="page">page to be removed</param>
        void RemovePage(Page page);

        /// <summary>
        /// Expense to be displayed
        /// </summary>
        /// <returns></returns>
        msdyn_expense GetExpense();

        bool IsValidExpense();

        Task<bool> Save();

        /// <summary>
        /// Check the behaviors of the new category, if there is data that needs to be deleted, 
        /// show a warning message to the user before change category.
        /// </summary>
        /// <param name="newExpenseCategory">expense category selected by the user</param>
        /// <param name="warningMessage">If there is data to delete, message with details of the data to be deleted</param>
        /// <returns>true if the change of category was successful</returns>
        Task<bool> SetCategory(msdyn_expensecategory newExpenseCategory, string warningMessage = null);
    }
}
