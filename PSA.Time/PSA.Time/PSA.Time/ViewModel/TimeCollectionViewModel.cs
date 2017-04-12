using Common.Model;
using Common.Utilities.DataAccess;
using Common.Utilities.Resources;
using Common.ViewModel;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using PSA.Time.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace PSA.Time.ViewModel
{
    public class TimeCollectionViewModel : BaseViewModel
    {
        /// <summary>
        /// Dictionary keyed by Date containing a TimeCollection for each.
        /// </summary>
        protected Dictionary<DateTime, TimeCollection> timesToDays;
        public static Dictionary<DateTime, int> DurationPerDay;

        private TimeCollectionDateFilter filter;

        /// <summary>
        /// Raised when a call to LoadTimes completes.
        /// </summary>
        public EventHandler LoadTimesCompleted;

        public TimeCollectionViewModel()
            : base()
        {
            // "My time"
            this.Title = AppResources.MyTime;

            DurationPerDay = new Dictionary<DateTime, int>();

            this.timesToDays = new Dictionary<DateTime, TimeCollection>();
            this.Days = new ObservableCollection<TimeCollection>();

            // New filter defaults to "this week"
            this.filter = new TimeCollectionDateFilter();
        }

        #region Properties

        public DateTime LastUpdatedDateTime { get; protected set; }

        /// <summary>
        /// All the TimeCollections sorted by date.
        /// </summary>
        public ObservableCollection<TimeCollection> Days { get; protected set; }

        /// <summary>
        /// Get the filter used to control the date range.
        /// </summary>
        public TimeCollectionDateFilter Filter
        {
            get
            {
                return this.filter;
            }
        }

        public override string Title
        {      
            get
            {
                return base.Title;
            }
            set
            {
                string result = value ?? string.Empty;
                if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.WinPhone)
                {
                    result = result.ToUpper();
                }

                base.Title = result;
            }
        }

        /// <summary>
        /// bool indicating if there are any time entries in the current view.
        /// </summary>
        public bool HasItems
        {
            get
            {
                bool result = false;

                if (this.Days != null && this.Days.Count > 0)
                {
                    result = true;
                }

                return result;
            }
        }

        private bool multiselectMode;
        /// <summary>
        /// Indicates if the view is in multiselect mode.
        /// </summary>
        public bool MultiselectModeEnabled
        {
            get
            {
                return multiselectMode;
            }
            set
            {
                if (multiselectMode != value)
                {
                    multiselectMode = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// Get from cache the list of Times and make an async call to the server for the updated list.
        /// </summary>
        public async Tasks.Task LoadTimes()
        {
            this.IsBusy = true;
            this.DataAccess.AccessMode = DataAccessMode.SyncConnection;

            // Prepare Query to Select Times
            QueryExpression retrieveTimeCollection = await this.buildTimeQueryExpression();

            // Get the data
            List<msdyn_timeentry> listTimes = await this.DataAccess.RetrieveEntities<msdyn_timeentry>(retrieveTimeCollection, null, true);

            if (listTimes != null)
            {
                this.LastUpdatedDateTime = DateTime.Now;
                this.ClearTimes();
                this.AddTimeEntries(listTimes);
            }
            this.NotifyPropertyChanged("HasItems");

            IsBusy = false;

            this.OnLoadTimesCompleted();
        }

        protected void OnLoadTimesCompleted()
        {
            if (this.LoadTimesCompleted != null)
            {
                this.LoadTimesCompleted(this, EventArgs.Empty);
            }
        }

        #region Load data from CRM

        /// <summary>
        /// Get the QueryExpression used to query CRM for time entries.
        /// </summary>
        /// <returns>A new QueryExpression.</returns>
        private async Tasks.Task<QueryExpression> buildTimeQueryExpression()
        {
            // Start the owner id retrieval
            Tasks.Task<ConditionExpression> ownerFilterTask = this.getOwnerFilter();

            // Build everything else for the QueryExpression
            QueryExpression retrieveTimeCollection = new QueryExpression(msdyn_timeentry.EntityLogicalName);
            retrieveTimeCollection.ColumnSet = new ColumnSet("msdyn_timeentryid", "msdyn_type", "msdyn_bookableresource", "msdyn_date", "msdyn_description", "msdyn_duration",
                "msdyn_entrystatus", "msdyn_externaldescription", "msdyn_project", "msdyn_projecttask", "modifiedon", "createdon");
            retrieveTimeCollection.Distinct = true;

            retrieveTimeCollection.Criteria = new FilterExpression();
            // Filter to selected month
            retrieveTimeCollection.Criteria.AddCondition(this.getCurrentPeriodFilter());

            // Order by time entry date
            this.setOrderBy(retrieveTimeCollection);

            // Let the owner filter finish before we return.
            retrieveTimeCollection.Criteria.AddCondition(await ownerFilterTask);

            return retrieveTimeCollection;
        }

        /// <summary>
        /// Create and return the ConditionExpression to filter by owner.
        /// </summary>
        /// <returns>ConditionExpression to filter Owner by currently logged in user.</returns>
        private async Tasks.Task<ConditionExpression> getOwnerFilter()
        {
            // Create filter for owner of the time entries.
            Guid userId = await this.DataAccess.GetLoggedUserId();
            BookableResource bookableResource = await TimeHelper.GetBookableResourceForUser(userId);
            Guid bookableResourceId = bookableResource != null ? bookableResource.BookableResourceId.GetValueOrDefault() : Guid.Empty;
            return new ConditionExpression(msdyn_timeentry.EntityLogicalName, "msdyn_bookableresource", ConditionOperator.Equal, bookableResourceId);
        }

        /// <summary>
        /// Create and return the ConditionExpress to filter by date.
        /// </summary>
        /// <returns>ConditionExpression filtering by date.</returns>
        private ConditionExpression getCurrentPeriodFilter()
        {
            // Specify the start and end date as UTC before converting to local time.
            DateTime startDateTime = DateTime.SpecifyKind(filter.StartDate, DateTimeKind.Utc).ToLocalTime(); 
            DateTime endDateTime = DateTime.SpecifyKind(filter.EndDate.AddMinutes(24 * 60 - 1), DateTimeKind.Utc).ToLocalTime();

            // Get all entries from beginning of the period to 23:59 end of the period in local time.
            return new ConditionExpression(msdyn_timeentry.EntityLogicalName, "msdyn_date", ConditionOperator.Between, new object[] { startDateTime, endDateTime });
        }

        private void setOrderBy(QueryExpression queryExpression)
        {
            queryExpression.Orders = new DataCollection<OrderExpression>();
            queryExpression.Orders.Add(new OrderExpression() { AttributeName = "msdyn_date", OrderType = OrderType.Descending });
        }

        #endregion

        private void addTimeEntryWithDateToTimeCollection(msdyn_timeentry entry, DateTime date)
        {
            TimeCollection timeCollection;

            // Get or create the correct TimeCollection
            if (this.timesToDays.ContainsKey(date))
            {
                timeCollection = this.timesToDays[date];
            }
            else
            {
                timeCollection = new TimeCollection(date);
                this.timesToDays.Add(date, timeCollection);
            }

            timeCollection.Add(entry);
        }

        private void addDurationPerDateToDictionary(msdyn_timeentry entry, DateTime date)
        {
            // If an entry for that day doesn't exist already, create and store the duration.
            if (!DurationPerDay.ContainsKey(date))
            {
                DurationPerDay.Add(date, (int)entry.msdyn_duration);
            }
            // Else, add up the duration with the tally of duration for the day.
            else
            {
                DurationPerDay[date] += (int)entry.msdyn_duration;
            }
        }

        public void AddTimeEntries(IEnumerable<msdyn_timeentry> times)
        {
            // Add all time entries with a valid date to the collection.
            foreach (msdyn_timeentry entry in times)
            {
                if (entry.msdyn_date != null)
                {
                    DateTime date = (DateTime)entry.msdyn_date;
                    this.addTimeEntryWithDateToTimeCollection(entry, date.Date);
                    this.addDurationPerDateToDictionary(entry, date.Date);
                }
            }

            this.populateDays(this.timesToDays.Keys);
        }

        /// <summary>
        /// Remove all entries from the collections.
        /// </summary>
        public void ClearTimes()
        {
            this.timesToDays = new Dictionary<DateTime, TimeCollection>();
            DurationPerDay = new Dictionary<DateTime, int>();
            this.Days.Clear();
        }

        private void populateDays(IEnumerable<DateTime> dateTimes)
        {
            this.Days.Clear();

            foreach (DateTime time in dateTimes.OrderBy(dt => { return dt; }))
            {
                this.Days.Add(this.timesToDays[time]);
            }
        }

        public void ClearSelection()
        {
            foreach (TimeCollection timeCollection in timesToDays.Values)
            {
                foreach (msdyn_timeentry entry in timeCollection)
                {
                    entry.Selected = false;
                }
            }
        }

        #region Actions

        /// <summary>
        /// Submit all the time entries that are selected.
        /// </summary>
        /// <returns></returns>
        public async Tasks.Task<bool> SubmitSelectedTimeEntries()
        {
            TimeEntrySubmitter submitter = new TimeEntrySubmitter();
            submitter.DataAccess = this.DataAccess;

            foreach (msdyn_timeentry timeEntry in this.SelectedEntries())
            {
                msdyn_timeentry_msdyn_entrystatus? status = timeEntry.EntryStatus;
                if (status == null || status.Value != msdyn_timeentry_msdyn_entrystatus.Submitted)
                {
                    submitter.addTimeEntry(timeEntry);
                }
            }

            return await submitter.ExecuteAction();
        }

        internal async Tasks.Task<bool> RecallSelectedTimeEntries()
        {
            TimeEntryRecaller recaller = new TimeEntryRecaller();
            recaller.DataAccess = this.DataAccess;

            foreach (msdyn_timeentry timeEntry in this.SelectedEntries())
            {
                msdyn_timeentry_msdyn_entrystatus? status = timeEntry.EntryStatus;
                if (status != null && status.Value == msdyn_timeentry_msdyn_entrystatus.Submitted)
                {
                    recaller.addTimeEntry(timeEntry);
                }
            }

            return await recaller.ExecuteAction();
        }

        #endregion

        private IEnumerable<msdyn_timeentry> SelectedEntries()
        {
            // Get all the selected entries
            return from tc in this.timesToDays.Values
                   from entry in tc
                   where entry.Selected
                   select entry;
        }

        /// <summary>
        /// Defaults the msdyn_date_utc field of the time entry provided based off the current date filter.
        /// </summary>
        /// <param name="timeEntry">The msdyn_timeentry record to default.</p</param>
        public void DefaultDateFromCurrentFilter(msdyn_timeentry timeEntry)
        {
            DateTime filterLastDate = filter.EndDate;
            DateTime utcToday = DateTime.UtcNow;

            timeEntry.msdyn_date_utc = DateTime.Compare(filterLastDate, utcToday) > 0 ? utcToday : filterLastDate;
        }

        /// <summary>
        /// Increment to the next period on the date filter and refresh.
        /// </summary>
        public async Tasks.Task IncrementDateFilter()
        {
            this.filter.Increment();

            // Refresh
            await this.LoadTimes();
        }

        /// <summary>
        /// Decrement to the previous period on the date filter and refresh.
        /// </summary>
        public async Tasks.Task DecrementDateFilter()
        {
            this.filter.Decrement();

            // Refresh
            await this.LoadTimes();
        }
    }
}
