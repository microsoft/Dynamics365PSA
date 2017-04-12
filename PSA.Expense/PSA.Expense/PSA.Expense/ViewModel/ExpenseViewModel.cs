using Common.Model;
using Common.Utilities;
using Common.Utilities.DataAccess;
using Common.Utilities.DataAccess.ServiceAccess;
using Common.Utilities.Extensions;
using Common.Utilities.Resources;
using Common.ViewModel;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSA.Expense.ViewModel
{
    public delegate Task<bool> SetCategoryDelegate(msdyn_expensecategory newExpenseCategory, string warningMessage = null);

    public class ExpenseViewModel : BaseEntityViewModel
    {
        public SetCategoryDelegate SetCategoryHandler { protected get; set; }

        protected msdyn_expense expense;
        public msdyn_expense Expense
        {
            get
            {
                return expense;
            }
            set
            {
                expense = value;
                NotifyPropertyChanged();
            }
        }

        protected FilterQuery[] ProjectFilters;
        protected int currentProjectFilter;

        public ExpenseViewModel(msdyn_expense expense)
            : base()
        {
            this.Expense = expense;
            this.Expense.PropertyChanged += Entity_PropertyChanged;
        }

        /// <summary>
        /// Try to unsuscribe of Expense.PropertyChanged
        /// </summary>
        ~ExpenseViewModel()
        {
            try
            {
                this.Expense.PropertyChanged -= Entity_PropertyChanged;
            }
            catch { }
        }

        /// <summary>
        /// If it is a new expense, set by default:
        /// 1. Today as Transaction date.
        /// 2. Draft as status
        /// 3. First available category 
        /// 4. First available currency
        /// </summary>
        public void SetDefaultsValues()
        {
            if (Expense.Id == Guid.Empty && Expense.CanEdit())
            {
                // Default transaction date to today
                Expense.msdyn_TransactionDate_utc = Expense.msdyn_TransactionDate_utc ?? DateTime.Today;

                // Default status to Draft
                Expense.msdyn_ExpenseStatus = Expense.msdyn_ExpenseStatus ?? new OptionSetValue((int)msdyn_expense_msdyn_expensestatus.Draft);

                // Default category to the first available one
                if (Expense.msdyn_ExpenseCategory == null)
                {
                    msdyn_expensecategory expenseCategory = this.GetDefaultData<msdyn_expensecategory>();
                    if (expenseCategory != null)
                    {
                        EntityReference newReference = new EntityReference(expenseCategory.LogicalName, expenseCategory.Id);
                        newReference.Name = expenseCategory.Preview;
                        Expense.msdyn_ExpenseCategory = newReference;
                    }
                }

                // Default transaction currency to the first available one.
                if (Expense.TransactionCurrency == null || Expense.TransactionCurrency.Id == Guid.Empty)
                {
                    TransactionCurrency transactionCurrency = this.GetDefaultData<TransactionCurrency>();
                    if (transactionCurrency != null)
                    {
                        Expense.TransactionCurrency = transactionCurrency;
                    }
                }
            }
        }

        /// <summary>
        /// If expense is not yet saved, save it first before submit.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> Submit()
        {
            if (this.CanEdit())
            {
                msdyn_expense expense = this.Expense;
                bool result = await this.Save();

                if (result)
                {
                    if (this.CanSubmit())
                    {
                        Common.Model.Actions.ActionRequest submitAction = new Common.Model.Actions.ActionRequest("msdyn_ExpenseEntriesSubmit");
                        submitAction.Parameters = new ParameterCollection();
                        submitAction.Parameters.Add(new KeyValuePair<string, object>("ExpenseEntryIds", String.Format("{0}", expense.Id)));
                        submitAction.Parameters.Add(new KeyValuePair<string, object>("NoteText", ""));
                        submitAction.Parameters.Add(new KeyValuePair<string, object>("CorrelationId", Guid.NewGuid().ToString()));
                        OrganizationResponse response = await this.DataAccess.Execute(submitAction);

                        if (response != null)
                        {
                            expense.msdyn_ExpenseStatus = new OptionSetValue((int)msdyn_expense_msdyn_expensestatus.Submitted);
                            return true;
                        }
                        else
                        {
                            await MessageCenter.ShowErrorMessage(AppResources.SubmitError);
                        }
                    }
                    else
                    {
                        await MessageCenter.ShowErrorMessage(AppResources.ExpenseReceiptSubmitError);
                    }
                }
                else
                {
                    await MessageCenter.ShowErrorMessage(AppResources.SaveError);
                }
            }
            return false;
        }

        /// <summary>
        /// Recall a submitted expense.
        /// </summary>
        /// <returns>True if recall succeeded, false otherwise.</returns>
        public virtual async Task<bool> Recall()
        {
            msdyn_expense expense = this.Expense;

            if (this.IsSubmitted())
            {
                Common.Model.Actions.ActionRequest recallAction = new Common.Model.Actions.ActionRequest("msdyn_ExpenseEntriesRecall");
                recallAction.Parameters = new ParameterCollection();
                recallAction.Parameters.Add(new KeyValuePair<string, object>("ExpenseEntryIds", String.Format("{0}", expense.Id)));
                recallAction.Parameters.Add(new KeyValuePair<string, object>("NoteText", ""));
                recallAction.Parameters.Add(new KeyValuePair<string, object>("CorrelationId", Guid.NewGuid().ToString()));
                OrganizationResponse response = await this.DataAccess.Execute(recallAction);

                if (response != null)
                {
                    expense.msdyn_ExpenseStatus = new OptionSetValue((int)msdyn_expense_msdyn_expensestatus.Draft);
                    return true;
                }
                else
                {
                    await MessageCenter.ShowErrorMessage(AppResources.RecallError);
                }
            }
            else
            {
                await MessageCenter.ShowErrorMessage(AppResources.RecallError);
            }
            return false;
        }

        #region CRUD Operations
        /// <summary>
        /// Save selected expense, if Id is null it will create a new one, 
        /// otherwise it will update the selected expense.
        /// </summary>
        /// <returns>True if the expense saved, false otherwise.</returns>
        public override async Task<bool> ForcedSave()
        {
            bool result = false;
            msdyn_expense expense = this.Expense;

            if (expense != null)
            {
                expense.msdyn_Amount = expense.msdyn_Amount ?? new Money(0);

                // If Expense can't be saved, don't allow the client to save, 
                // return false so user can make the corresponding corrections.
                if (this.CanExpenseBeSaved(expense))
                {
                    // If expense doesn't have an Id, it need to be created
                    if (expense.Id == null || expense.Id == Guid.Empty)
                    {
                        // Always clear the expense id when creating a new entry
                        // because CRM validation treats Guid.Empty as an invalid id for a new record.
                        Guid? expenseId = await this.DataAccess.Create(expense);

                        if (expenseId != null && expenseId != Guid.Empty)
                        {
                            expense.Id = (Guid)expenseId;
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = await this.DataAccess.Update(expense);
                    }

                    // Update status of expense.
                    if (this.HasPendingDataToSave)
                    {
                        this.HasPendingDataToSave = !result;
                    }

                    // If save failed due to something unforseen, show a message.
                    if (!result)
                    {
                        await MessageCenter.ShowErrorMessage(AppResources.SaveError);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// If the expense is not yet created, just delete from memory. Otherwise delete from server
        /// </summary>
        /// <param name="omitWarningMessage">True if deletion would happen without a confirmation from the user</param>
        /// <returns>True if the expense was deleted</returns>
        public async Task<bool> Delete(bool omitWarningMessage = false)
        {
            msdyn_expense expense = this.Expense;

            if (expense != null)
            {
                if (expense.Id != null && expense.Id != Guid.Empty)
                {
                    if (omitWarningMessage || await MessageCenter.ShowDialog(AppResources.DeleteWarningExpense, null, null))
                    {
                        return await this.DataAccess.Delete(expense.LogicalName, expense.Id);
                    }
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Validation methods
        /// <summary>
        /// Validates if an expense can be saved.
        /// </summary>
        /// <param name="expense">The expense for which to validate save.</param>
        /// <returns>True if it can be saved, false otherwise.</returns>
        public virtual bool CanExpenseBeSaved(msdyn_expense expense)
        {
            if (expense.msdyn_ExpenseCategory == null)
            {
                MessageCenter.ShowErrorMessage(AppResources.CategoryError).DoNotAwait();
                return false;
            }
            return true;
        }

        /// <summary>
        /// If the expense is in draft or rejected status it can be saved
        /// </summary>
        /// <returns></returns>
        internal bool CanEdit()
        {
            return (this.Expense != null && this.Expense.CanEdit());
        }

        /// <summary>
        /// Validates if the expense can be submitted. 
        /// </summary>
        /// <returns>True if it can be submitted, false otherwise.</returns>
        internal bool CanSubmit()
        {
            bool result = false;
            msdyn_expense currentExpense = this.Expense;

            if (currentExpense != null && currentExpense.ExpenseCategory != null)
            {
                if (currentExpense.ExpenseCategory.IsReceiptMandatory())
                {
                    return currentExpense.HasReceipts;
                }
                else
                {
                    return true;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if an expense is submitted. 
        /// </summary>
        /// <returns>True if the expense is submitted, false otherwise.</returns>
        internal bool IsSubmitted()
        {
            if (this.Expense == null || this.Expense.msdyn_ExpenseStatus == null)
            {
                return false;
            }

            return (this.Expense.msdyn_ExpenseStatus.Value == (int)msdyn_expense_msdyn_expensestatus.Submitted);
        }
        #endregion

        #region OnSelectedMethods
        /// <summary>
        /// Search in the ReferenceData dictionary the object for given type and key
        /// and added to selected Expense
        /// </summary>
        /// <param name="key">selected item from UI</param>
        public async Task<bool> OnProjectSelected(string selectedItemKey, int selectedItemIndex)
        {
            msdyn_project selectedProject = this.GetObjectByName<msdyn_project>(selectedItemKey);

            if (selectedProject != null)
            {
                if (Expense.msdyn_Project == null || selectedProject.Id != Expense.msdyn_Project.Id)
                {
                    EntityReference newReference = new EntityReference(msdyn_project.EntityLogicalName, selectedProject.Id);
                    Expense.msdyn_Project = newReference;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Search in the ReferenceData dictionary the object for given type and key
        /// and added to selected Expense
        /// </summary>
        /// <param name="key">selected item from UI</param>
        public async Task<bool> OnCategorySelected(string selectedItemKey, int selectedItemIndex)
        {
            msdyn_expensecategory selectedCategory = this.GetObjectByName<msdyn_expensecategory>(selectedItemKey);

            if (selectedCategory != null)
            {
                if (Expense.ExpenseCategory != null && selectedCategory.Id == Expense.ExpenseCategory.Id)
                {
                    // It is not a change
                    return true;
                }
                else if (this.SetCategoryHandler != null)
                {
                    // Allow all the behaviors to react based on new category
                    return await this.SetCategoryHandler(selectedCategory);
                }
                else
                {
                    Expense.ExpenseCategory = selectedCategory;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> OnCurrencySelected(string selectedItemKey, int selectedItemIndex)
        {
            TransactionCurrency selectedCurrency = this.GetObjectByName<TransactionCurrency>(selectedItemKey);
            if (selectedCurrency != null)
            {
                if (Expense.TransactionCurrency == null || selectedCurrency.Id != Expense.TransactionCurrency.Id)
                {
                    Expense.TransactionCurrency = selectedCurrency;
                }
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Load data for projects, categories and currencies.
        /// </summary>
        /// <returns></returns>
        public override async System.Threading.Tasks.Task LoadReferenceData()
        {
            // Load the data in all cases, otherwise some platforms won't show the controls
            // if data bound to the control is not present.
            this.IsBusy = true;
            this.DataAccess.AccessMode = DataAccessMode.SyncConnection;
            // Retrieve expense categories
            QueryExpression queryExpression = new QueryExpression(msdyn_expensecategory.EntityLogicalName);
            queryExpression.ColumnSet = new ColumnSet("msdyn_expensecategoryuid", "msdyn_expensecategoryid", "msdyn_expensetype", "msdyn_receiptrequired", "msdyn_name");
            await this.LoadReferenceData<msdyn_expensecategory>(queryExpression);

            // Retrieve Currencies
            queryExpression.LinkEntities.Clear();
            queryExpression.EntityName = TransactionCurrency.EntityLogicalName;
            queryExpression.ColumnSet = new ColumnSet("transactioncurrencyid", "currencyname", "isocurrencycode", "currencysymbol",
                "exchangerate", "currencyprecision");
            await this.LoadReferenceData<TransactionCurrency>(queryExpression);

            queryExpression.EntityName = msdyn_project.EntityLogicalName;
            queryExpression.ColumnSet = new ColumnSet("msdyn_projectid", "msdyn_subject", "msdyn_description");

            // Retrieve projects based on project team
            LinkEntity projectTeamLink = queryExpression.AddLink(msdyn_projectteam.EntityLogicalName, "msdyn_projectid", "msdyn_project", JoinOperator.Inner);
            projectTeamLink.Columns.AddColumn("msdyn_bookableresourceid");
            projectTeamLink.EntityAlias = "msdyn_projectteam";

            Guid userId = await this.DataAccess.GetLoggedUserId(true);
            BookableResource bookableResource = await ExpenseHelper.GetBookableResourceForUser(userId);
            Guid bookableResourceId = bookableResource != null ? bookableResource.BookableResourceId.GetValueOrDefault() : Guid.Empty;
            bool searchAllProjects = true;
            ProjectFilters = new FilterQuery[2];

            if (userId != Guid.Empty)
            {
                ConditionExpression crmUserExpression = new ConditionExpression(msdyn_projectteam.EntityLogicalName, "msdyn_bookableresourceid", ConditionOperator.Equal, bookableResourceId);
                queryExpression.Criteria = new FilterExpression();
                queryExpression.Criteria.AddCondition(crmUserExpression);
                searchAllProjects = !await this.LoadReferenceData<msdyn_project>(queryExpression);
                currentProjectFilter = 0;
                ProjectFilters[currentProjectFilter] = new FilterQuery(AppResources.ShowMyProjects, queryExpression, this.GetReferenceKeys<msdyn_project>());
            }

            if (searchAllProjects)
            {
                // If my projects didn't return values or I don't have the current user id, query all projects
                queryExpression.Criteria = null;
                await this.LoadReferenceData<msdyn_project>(queryExpression);
                currentProjectFilter = 1;
                ProjectFilters[currentProjectFilter] = new FilterQuery(AppResources.ShowAllProjects, queryExpression, this.GetReferenceKeys<msdyn_project>());
            }
            else
            {
                ProjectFilters[1] = new FilterQuery(AppResources.ShowAllProjects, queryExpression, null);
            }
            this.IsBusy = false;
        }

        /// <summary>
        /// Return the keys of the reference data ensuring current data in model is present
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ICollection<string> GetReferenceKeys<TEntity>(string attributeLogicalName) where TEntity : Entity, new()
        {
            if (attributeLogicalName != null)
            {
                EntityReference currentReference = this.Expense.GetAttributeValue<EntityReference>(attributeLogicalName);

                if (currentReference != null)
                {
                    TEntity currentData = new TEntity();
                    currentData.LogicalName = currentReference.LogicalName;
                    currentData.Id = currentReference.Id ?? Guid.Empty;
                    currentData.Preview = currentReference.Name;

                    if (currentData.Id != Guid.Empty && this.GetObjectById<TEntity>(currentData.Id) == null)
                    {
                        Common.Model.Extension.ReferenceData referenceData = this.GetOrCreateReferenceData<TEntity>();
                        referenceData.AddData(currentData);
                    }
                }
            }
            return base.GetReferenceKeys<TEntity>();
        }

        /// <summary>
        /// It will rotate between all the available filters of project and return the next one.
        /// If the limit is reached it will started from the beginning
        /// </summary>
        /// <returns></returns>
        public async Task<FilterQuery> GetFilterProject(bool switchFilter = false)
        {
            FilterQuery filter = null;

            if (ProjectFilters != null)
            {
                if (switchFilter)
                {
                    currentProjectFilter++;
                    if (currentProjectFilter >= this.ProjectFilters.Length)
                    {
                        currentProjectFilter = 0;
                    }
                }

                filter = ProjectFilters[currentProjectFilter];

                if (filter != null && filter.Result == null
                    && await this.LoadReferenceData<msdyn_project>(filter.QueryExpression))
                {
                    filter.Result = this.GetReferenceKeys<msdyn_project>();
                }
            }
            return filter;
        }
    }
}
