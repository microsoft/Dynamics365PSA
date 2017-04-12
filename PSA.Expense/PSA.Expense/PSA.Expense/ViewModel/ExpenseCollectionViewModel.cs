using Common.Model;
using Common.Utilities.DataAccess;
using Common.Utilities.Resources;
using Common.ViewModel;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PSA.Expense.ViewModel
{
    public class ExpenseCollectionViewModel : BaseViewModel
    {
        public DateTime LastUpdatedDateTime { get; protected set; }

        /// <summary>
        /// All the ExpensesCollection.
        /// </summary>
        public ObservableCollection<msdyn_expense> ExpensesCollection { get; protected set; }

        /// <summary>
        /// Raised when a call to LoadExpenses completes.
        /// </summary>
        public EventHandler LoadExpensesCompleted;

        public enum ExpenseStatus { All, Draft, Rejected, Pending };

        public ExpenseStatus currentExpenseStatus;

        public ExpenseCollectionViewModel() : base()
        {
            this.Title = AppResources.MyExpenses;
            this.ExpensesCollection = new ObservableCollection<msdyn_expense>();
            this.currentExpenseStatus = ExpenseStatus.Draft;
        }

        /// <summary>
        /// Get from cache the list of expenses and make an async call to the server for the updated list.
        /// Get first 30 expenses for the "All" list
        /// </summary>
        public async System.Threading.Tasks.Task LoadExpenses()
        {
            this.IsBusy = true;
            this.DataAccess.AccessMode = DataAccessMode.SyncConnection;
            
            // Prepare Query to Select expenses
            QueryExpression retrieveExpenseCollection = new QueryExpression(msdyn_expense.EntityLogicalName);
            retrieveExpenseCollection.ColumnSet = new ColumnSet("msdyn_expensestatus", "msdyn_expenseid", "msdyn_transactiondate", "msdyn_bookableresource",
                "msdyn_expensecategory", "msdyn_project", "msdyn_name", "msdyn_salestaxamount", "msdyn_amount", "transactioncurrencyid", "modifiedon", "createdon");
            retrieveExpenseCollection.Distinct = true;

            // Filter by owner id
            Guid ownerId = await this.GetOwnerId();
            BookableResource bookableResource = await ExpenseHelper.GetBookableResourceForUser(ownerId);
            Guid bookableResourceId = bookableResource != null ? bookableResource.BookableResourceId.GetValueOrDefault() : Guid.Empty;
            ConditionExpression crmUserExpression = new ConditionExpression(msdyn_expense.EntityLogicalName, "msdyn_bookableresource", ConditionOperator.Equal, bookableResourceId);
            retrieveExpenseCollection.Criteria = new FilterExpression();
            retrieveExpenseCollection.Criteria.AddCondition(crmUserExpression);

            // Order by createdon date
            retrieveExpenseCollection.Orders = new DataCollection<OrderExpression>();
            retrieveExpenseCollection.Orders.Add(new OrderExpression() { AttributeName = "createdon", OrderType = OrderType.Descending });

            // Check if the expense has notes
            LinkEntity notesLink = retrieveExpenseCollection.AddLink(Annotation.EntityLogicalName, "msdyn_expenseid", "objectid", JoinOperator.LeftOuter);
            notesLink.Columns.AddColumns("objectid");
            notesLink.EntityAlias = "notes";

            // Check if the expense has receipts
            LinkEntity receiptsLink = retrieveExpenseCollection.AddLink(msdyn_expensereceipt.EntityLogicalName, "msdyn_expenseid", "msdyn_expenseid", JoinOperator.LeftOuter);
            receiptsLink.Columns.AddColumns("msdyn_expensereceiptid");
            receiptsLink.EntityAlias = "receipts";

            // An entry in msdyn_expenseReceipt is not enough to determine if there are receipts, so adding a left outer with annotation
            LinkEntity linkEntity = new LinkEntity()
            {
                LinkToEntityName = Annotation.EntityLogicalName,
                LinkFromEntityName = msdyn_expensereceipt.EntityLogicalName,
                LinkFromAttributeName = "msdyn_expensereceiptid",
                LinkToAttributeName = "objectid",
                JoinOperator = JoinOperator.LeftOuter,
            };

            linkEntity.Columns.AddColumns("objectid");
            linkEntity.EntityAlias = "receiptsExtended";
            receiptsLink.LinkEntities.Add(linkEntity);

            // Get Currency Symbol currencysymbol
            LinkEntity transactionCurrencyLink = retrieveExpenseCollection.AddLink(TransactionCurrency.EntityLogicalName, "transactioncurrencyid", "transactioncurrencyid", JoinOperator.LeftOuter);
            transactionCurrencyLink.Columns.AddColumns("isocurrencycode");
            transactionCurrencyLink.EntityAlias = "transactioncurrency";

            // Get Expense type
            LinkEntity expenseCategoryLink = retrieveExpenseCollection.AddLink(msdyn_expensecategory.EntityLogicalName, "msdyn_expensecategory", "msdyn_expensecategoryid", JoinOperator.LeftOuter);
            expenseCategoryLink.Columns.AddColumns("msdyn_expensetype", "msdyn_receiptrequired");
            expenseCategoryLink.EntityAlias = msdyn_expensecategory.EntityLogicalName;

            // Clear previous results
            this.ExpensesCollection.Clear();
            this.LastUpdatedDateTime = DateTime.Now;

            // Get all expenses.
            switch (this.currentExpenseStatus)
            {
                // All expenses.
                case ExpenseStatus.All: break;

                // Draft expenses.
                case ExpenseStatus.Draft:
                    ConditionExpression statusExpressionDraft = new ConditionExpression(msdyn_expense.EntityLogicalName, "msdyn_expensestatus", 
                                                                                   ConditionOperator.Equal, (int)msdyn_expense_msdyn_expensestatus.Draft);
                    retrieveExpenseCollection.Criteria.AddCondition(statusExpressionDraft);
                    break;

                // Rejected expenses.
                case ExpenseStatus.Rejected:
                    ConditionExpression statusExpressionRejected = new ConditionExpression(msdyn_expense.EntityLogicalName, "msdyn_expensestatus",
                                                                                   ConditionOperator.Equal, (int)msdyn_expense_msdyn_expensestatus.Rejected);
                    retrieveExpenseCollection.Criteria.AddCondition(statusExpressionRejected);
                    break;

                // All "pending" expenses
                case ExpenseStatus.Pending:
                    ConditionExpression statusExpressionPending = new ConditionExpression(msdyn_expense.EntityLogicalName, "msdyn_expensestatus", ConditionOperator.In);
                    statusExpressionPending.Values.Add((int)msdyn_expense_msdyn_expensestatus.Submitted);
                    statusExpressionPending.Values.Add((int)msdyn_expense_msdyn_expensestatus.Approved);
                    statusExpressionPending.Values.Add((int)msdyn_expense_msdyn_expensestatus.Posted);
                    retrieveExpenseCollection.Criteria.AddCondition(statusExpressionPending);
                    break;

                default:
                    break;
            }

            List<msdyn_expense> listExpenses = await this.DataAccess.RetrieveEntities<msdyn_expense>(retrieveExpenseCollection, null, true);

            if (listExpenses != null)
            {
                foreach (msdyn_expense expense in listExpenses)
                {
                    this.ExpensesCollection.Add(expense);
                }
            }

            this.OnLoadExpensesCompleted();
            this.IsBusy = false;
        }

        public async Task<bool> OnExpenseStatusSelected(string selectedItemKey, int selectedItemIndex)
        {
            if (!String.IsNullOrEmpty(selectedItemKey))
            {
                this.currentExpenseStatus = (ExpenseStatus)Enum.Parse(typeof(ExpenseStatus), selectedItemKey);
                await this.LoadExpenses();
                return true;                                  
            }
            return false;
        }

        protected void OnLoadExpensesCompleted()
        {
            if (this.LoadExpensesCompleted != null)
            {
                this.LoadExpensesCompleted(this, EventArgs.Empty);
            }
        }    

        /// <summary>
        /// If there is no delegation, get current user id
        /// </summary>
        /// <returns></returns>
        protected async Task<Guid> GetOwnerId()
        {
            return await this.DataAccess.GetLoggedUserId();
        }
    }
}
