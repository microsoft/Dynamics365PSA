using Common.Model;
using Microsoft.Xrm.Sdk.Samples;

namespace PSA.Time.ViewModel
{
    /// <summary>
    /// Class to help execute Submit action for multiple time entries.
    /// </summary>
    public class TimeEntrySubmitter : TimeEntryAction
    {
        public const string TimeEntriesSubmitActionName = "msdyn_TimeEntriesSubmit";
        
        public TimeEntrySubmitter() : base()
        {
        }
    
        protected override void updateTimeEntriesStatus()
        {
            foreach (msdyn_timeentry timeEntry in this.Entries)
            {
                timeEntry.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Submitted);
            }
        }

        protected override string getActionName()
        {
            return TimeEntrySubmitter.TimeEntriesSubmitActionName;
        }
    }
}
