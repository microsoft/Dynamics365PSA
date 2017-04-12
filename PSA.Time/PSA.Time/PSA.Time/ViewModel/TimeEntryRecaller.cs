using Common.Model;
using Microsoft.Xrm.Sdk.Samples;

namespace PSA.Time.ViewModel
{
    /// <summary>
    /// Class to help execute Recall action for multiple time entries.
    /// </summary>
    public class TimeEntryRecaller : TimeEntryAction
    {
        public const string RecallActionName = "msdyn_TimeEntriesRecall";
        public const string NoteParameterName = "Notes";

        protected override string getActionName()
        {
            return TimeEntryRecaller.RecallActionName;
        }

        protected override void updateTimeEntriesStatus()
        {
            foreach (msdyn_timeentry timeEntry in this.Entries)
            {
                timeEntry.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Draft);
            }
        }

        protected override string getNoteTextParameterName()
        {
            return TimeEntryRecaller.NoteParameterName;
        }
    }
}
