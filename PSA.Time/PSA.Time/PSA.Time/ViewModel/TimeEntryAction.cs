using Common.Model;
using Common.Model.Actions;
using Common.Utilities.DataAccess;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PSA.Time.ViewModel
{
    /// <summary>
    /// Base class for executing actions against time entries.
    /// </summary>
    public abstract class TimeEntryAction
    {
        public const string TimeEntryIdsParameterName = "TimeEntryIds";
        public const string NoteTextParameterName = "NoteText";
        public const string CorrelationIdName = "CorrelationId";

        /// <summary>
        /// Get the name of the note parameter.
        /// </summary>
        /// <returns>string name of the parameter.</returns>
        protected virtual string getNoteTextParameterName()
        {
            return TimeEntryAction.NoteTextParameterName;
        }

        /// <summary>
        /// Get the name of the correlation Id parameter.
        /// </summary>
        /// <returns>String name of the parameter.</returns>
        protected virtual string getCorrelationIdParameterName()
        {
            return TimeEntryAction.CorrelationIdName;
        }

        // entries to execute action on
        private List<msdyn_timeentry> entries;

        /// <summary>
        /// The list of msdyn_timeentry to execute the action against.
        /// </summary>
        protected IEnumerable<msdyn_timeentry> Entries
        {
            get
            {
                return entries;
            }
        }

        /// <summary>
        /// Get or set the DataAccess class used execute the action.
        /// </summary>
        public DataAccess DataAccess { get; set; }

        /// <summary>
        /// Get or set the note to be included in the action.
        /// </summary>
        public string NoteText { get; set; }

        public TimeEntryAction()
        {
            this.entries = new List<msdyn_timeentry>();
            this.NoteText = string.Empty;
        }

        /// <summary>
        /// Add a time entry.
        /// </summary>
        /// <param name="timeEntry"></param>
        public void addTimeEntry(msdyn_timeentry timeEntry)
        {
            entries.Add(timeEntry);
        }

        /// <summary>
        /// Submit the time entries.
        /// </summary>
        /// <returns>true if the submission succeeded; otherwise, false.</returns>
        public async Task<bool> ExecuteAction()
        {
            ActionRequest action = new ActionRequest(this.getActionName());

            action.Parameters = new ParameterCollection();
            action.Parameters.Add(TimeEntryAction.TimeEntryIdsParameterName, this.buildEntryIdString());
            action.Parameters.Add(this.getNoteTextParameterName(), this.NoteText);
            action.Parameters.Add(this.getCorrelationIdParameterName(), Guid.NewGuid().ToString());

            OrganizationResponse response = await this.DataAccess.Execute(action);

            if (response != null)
            {
                this.updateTimeEntriesStatus();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Change msdyn_entryStatus for all the entries.
        /// </summary>
        protected abstract void updateTimeEntriesStatus();

        /// <summary>
        /// Get the name of the action to execute
        /// </summary>
        /// <returns></returns>
        protected abstract string getActionName();

        /// <summary>
        /// Create a comma seperated list of the time entry IDs
        /// </summary>
        /// <returns></returns>
        private string buildEntryIdString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < entries.Count; i++)
            {
                sb.Append(entries[i].Id);

                if (i + 1 != entries.Count)
                {
                    sb.Append(',');
                }
            }

            return sb.ToString();
        }

    }
}
