using Common.Model;
using Common.Utilities;
using Common.Utilities.Resources;
using Common.ViewModel;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PSA.Expense.ViewModel
{
    public class NoteViewModel : BaseViewModel
    {
        public ObservableCollection<Annotation> AttachedNotes { get; protected set; }
        public msdyn_expense SelectedExpense { get; set; }

        public NoteViewModel(msdyn_expense expense)
            : base()
        {
            this.Title = AppResources.Receipts;
            this.AttachedNotes = new ObservableCollection<Annotation>();
            this.SelectedExpense = expense;
        }

        /// <summary>
        /// Return true if the expense has comments
        /// </summary>
        /// <returns></returns>
        public virtual bool HasNotes(bool? hasNotes = null)
        {
            if(hasNotes != null)
            {
                this.SelectedExpense.HasComments = hasNotes ?? false;
            }
            return this.SelectedExpense.HasComments;
        }


        /// <summary>
        /// Gets the condition that represents the relation between the expense and the notes
        /// </summary>
        /// <returns></returns>
        protected virtual ConditionExpression GetExpenseConditionExpression()
        {
            // Filter by expense id
            return new ConditionExpression(Annotation.EntityLogicalName, "objectid", ConditionOperator.Equal, SelectedExpense.Id);
        }

        public async System.Threading.Tasks.Task Initialize()
        {
            // Clear previous results
            this.AttachedNotes.Clear();

            if (SelectedExpense != null && this.HasNotes())
            {
                // Select notes
                QueryExpression retrieveAnnotationCollection = new QueryExpression(Annotation.EntityLogicalName);
                retrieveAnnotationCollection.ColumnSet = new ColumnSet(new string[] { "annotationid", "filename", "mimetype", "notetext", "createdon" });

                ConditionExpression crmExpenseExpression = GetExpenseConditionExpression();
                retrieveAnnotationCollection.Criteria = new FilterExpression();
                retrieveAnnotationCollection.Criteria.AddCondition(crmExpenseExpression);

                // Order by createdon date
                retrieveAnnotationCollection.Orders = new DataCollection<OrderExpression>();
                retrieveAnnotationCollection.Orders.Add(new OrderExpression() { AttributeName = "createdon", OrderType = OrderType.Ascending });

                List<Annotation> attachments = await this.DataAccess.RetrieveEntities<Annotation>(retrieveAnnotationCollection);
                if (attachments != null)
                {
                    foreach (var attach in attachments)
                    {
                        AttachedNotes.Add(attach);
                    }
                }
            }
        }

        /// <summary>
        /// Get the document body of the receipt
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<Annotation> GetAdditionalInformation(Annotation note)
        {
            if (note != null && note.Id != Guid.Empty)
            {
                ColumnSet columnsAnnotation = new ColumnSet(new string[] { "annotationid", "documentbody" });
                Annotation retrievedNote = await this.DataAccess.RetrieveEntity<Annotation>(Annotation.EntityLogicalName, note.Id, columnsAnnotation);
                if (retrievedNote != null && note.Id == retrievedNote.Id)
                {
                    note.DocumentBody = retrievedNote.DocumentBody;
                }
            }
            return note;
        }

        /// <summary>
        /// Delete a note given the id
        /// </summary>
        /// <param name="noteId">Id of the note to delete</param>
        /// <returns>True if the note was deleted</returns>
        protected async System.Threading.Tasks.Task<bool> DeleteFromServer(Guid noteId)
        {
            if (noteId != Guid.Empty && SelectedExpense.CanEdit())
            {
                // Delete from server
                return await this.DataAccess.Delete(Annotation.EntityLogicalName, noteId);
            }
            return false;
        }

        /// <summary>
        /// Delete a note given the id and update in memory representation
        /// </summary>
        /// <param name="noteId">Id of the receipt to delete</param>
        /// <param name="noteId"></param>
        /// <param name="omitWarningMessage">True if deletion would happen without a confirmation from the user</param>
        /// <returns>True if the receipt was deleted</returns>
        public async System.Threading.Tasks.Task Delete(Guid noteId, bool omitWarningMessage = false)
        {
            this.IsBusy = true;
            if (omitWarningMessage || await MessageCenter.ShowDialog(AppResources.DeleteWarning, null, null))
            {
                if (await this.DeleteFromServer(noteId))
                {
                    for (int i = 0; i < this.AttachedNotes.Count; i++)
                    {
                        // Delete local object 
                        Annotation receipt = this.AttachedNotes[i];
                        if (receipt != null && receipt.Id == noteId)
                        {
                            this.AttachedNotes.RemoveAt(i);
                            break;
                        }
                    }
                    this.HasNotes(this.AttachedNotes.Count > 0);
                }
                else
                {                    
                    await MessageCenter.ShowErrorMessage(AppResources.errorRestCall);
                }
            }
            this.IsBusy = false;
        }

        /// <summary>
        /// Delete all notes
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteAll()
        {
            if (SelectedExpense.CanEdit())
            {
                this.IsBusy = true;
                int count = this.AttachedNotes.Count - 1;
                
                while (count >= 0)
                {
                    // Delete local object 
                    Annotation receipt = this.AttachedNotes[count];
                    if (receipt != null && await this.DeleteFromServer(receipt.Id))
                    {
                        this.AttachedNotes.RemoveAt(count);
                    }
                    count--;
                }

                // If there is at least one receipt, show error
                if (count > 0)
                {                    
                    await MessageCenter.ShowErrorMessage(AppResources.errorRestCall);
                }
                else
                {
                    this.HasNotes(false);
                }

                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Add a note to the selected expense
        /// </summary>
        /// <param name="imgArray"></param>
        internal async System.Threading.Tasks.Task<bool> AddNote(string noteText)
        {
            if (SelectedExpense.CanEdit() && !String.IsNullOrEmpty(noteText)
                && this.SelectedExpense.Id != null && this.SelectedExpense.Id != Guid.Empty) 
            {   
                Annotation note = new Annotation()
                {
                    NoteText = noteText,
                    ObjectId = new EntityReference(this.SelectedExpense.LogicalName, this.SelectedExpense.Id)
                };
                return await this.SaveNote(note);
            }
            return false;
        }

        protected async System.Threading.Tasks.Task<bool> SaveNote(Annotation note)
        {
            if (note != null)
            {
                Guid newNoteId = await this.DataAccess.Create(note) ?? Guid.Empty;
                if (newNoteId != Guid.Empty)
                {
                    note.Id = newNoteId;
                    this.AttachedNotes.Add(note);
                    return true;
                }
            }
            return false;
        }
    }
}
