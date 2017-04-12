using Common.Model;
using Common.Utilities.Resources;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;

namespace PSA.Expense.ViewModel
{
    public class ReceiptViewModel : NoteViewModel
    {
        public ReceiptViewModel(msdyn_expense expense) :base(expense)
        {
            this.Title = AppResources.Receipts;
        }

        /// <summary>
        /// Return true if the expense has receipts
        /// </summary>
        /// <returns></returns>
        public override bool HasNotes(bool? hasNotes = null)
        {
            if (hasNotes != null)
            {
                this.SelectedExpense.HasReceipts = hasNotes ?? false;
            }
            return this.SelectedExpense.HasReceipts;
        }

        /// <summary>
        /// Gets the condition that represents the relation between the expense and the receipts
        /// </summary>
        /// <returns></returns>
        protected override ConditionExpression GetExpenseConditionExpression()
        {
            // Filter by expense receipt id
            return new ConditionExpression(Annotation.EntityLogicalName, "objectid", ConditionOperator.Equal, SelectedExpense.ExpenseReceiptId);
        }

        /// <summary>
        /// Add a receipt to the selected expense.
        /// </summary>
        /// <param name="receiptImage">The byte array containing the receipt image.</param>
        internal async System.Threading.Tasks.Task<bool> AddReceipt(byte[] receiptImage)
        {
            if (this.SelectedExpense.CanEdit() && receiptImage != null)
            {
                // If it is the first receipt of the expense, create an expense-receipt link.
                if (this.SelectedExpense.ExpenseReceiptId == null || this.SelectedExpense.ExpenseReceiptId == Guid.Empty)
                { 
                    msdyn_expensereceipt expenseReceipt = new msdyn_expensereceipt();
                    expenseReceipt.msdyn_ExpenseId = new EntityReference(this.SelectedExpense.LogicalName, this.SelectedExpense.Id);
                    this.SelectedExpense.ExpenseReceiptId = await this.DataAccess.Create(expenseReceipt) ?? Guid.Empty;
                }

                // Instantiate an Annotation object with the given image
                Annotation receipt = new Annotation()
                {
                    MimeType = @"image/jpeg",
                    Subject = "Expense Receipt",
                    FileName = String.Format("ExpenseAttachment.jpeg"),
                    DocumentBody = Convert.ToBase64String(receiptImage),
                    ObjectId = new EntityReference(msdyn_expensereceipt.EntityLogicalName, this.SelectedExpense.ExpenseReceiptId)
                };

                if (await this.SaveNote(receipt))
                {
                    this.SelectedExpense.HasReceipts = true;
                    return true;
                }
            }
            return false;
        }
    }
}
