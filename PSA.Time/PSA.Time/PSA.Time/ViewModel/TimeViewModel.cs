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
using Tasks = System.Threading.Tasks;

namespace PSA.Time.ViewModel
{
    public delegate Tasks.Task<bool> SetCategoryDelegate(msdyn_expensecategory newTimeCategory, string warningMessage = null);

    public class TimeViewModel : BaseEntityViewModel
    {
        public SetCategoryDelegate SetCategoryHandler { protected get; set; }

        protected msdyn_timeentry time;
        public msdyn_timeentry Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                NotifyPropertyChanged();
            }
        }

        protected FilterQuery[] ProjectFilters;
        protected int currentProjectFilter;

        // ProjectFilters[0] stores the filter query needed for filtering projects owned by current user.
        // ProjectFilters[1] stores the filter query needed to query all projects.
        private const int MyProjectFilterIndex = 0;
        private const int AllProjectsFilterIndex = 1;        

        public TimeViewModel(msdyn_timeentry time)
            : base()
        {
            this.Time = time;
            this.Time.PropertyChanged += Entity_PropertyChanged;
        }

        /// <summary>
        /// Try to unsuscribe of Time.PropertyChanged
        /// </summary>
        ~TimeViewModel()
        {
            try
            {
                this.Time.PropertyChanged -= Entity_PropertyChanged;
            }
            catch { }
        }

        /// <summary>
        /// If it is a new Time, set by default:
        /// 1. Today as date.
        /// 2. Draft as status
        /// 3. Duration as 60 minutes 
        /// 4. Type as work
        /// </summary>
        public void SetDefaultValues()
        {
            if (Time.Id == Guid.Empty)
            {
                // Default date is today
                Time.msdyn_date_utc = Time.msdyn_date_utc ?? DateTime.UtcNow;

                // Default duration is 1 hour (60 minutes)
                Time.msdyn_duration = 60;

                // Default type is Work
                Time.msdyn_type = Time.msdyn_type ?? new OptionSetValue((int)msdyn_timeentrytype.Work);

                // Default status to Draft
                Time.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Draft);
            }
        }

        #region Actions
        
        /// <summary>
        /// Submit the time entry.
        /// </summary>
        /// <returns>true if the submission succeeded; otherwise, false.</returns>
        public virtual async Tasks.Task<bool> Submit()
        {
            if (this.CanEdit())
            {
                msdyn_timeentry time = this.Time;
                bool result =  await this.Save();
                
                if (result)
                {
                    TimeEntrySubmitter submitter = new TimeEntrySubmitter();
                    submitter.DataAccess = this.DataAccess;
                    submitter.addTimeEntry(this.Time);
                    
                    return await submitter.ExecuteAction();                 
                }
                else
                {
                    await MessageCenter.ShowErrorMessage(AppResources.SaveError);
                }
            }
            return false;
        }

        /// <summary>
        /// Recall a time entry.
        /// </summary>
        /// <returns></returns>
        public virtual async Tasks.Task<bool> Recall()
        {
            msdyn_timeentry time = this.Time;

            if (!this.IsSubmitted())
            {
                return false;
            }

            TimeEntryRecaller recaller = new TimeEntryRecaller();
            recaller.DataAccess = this.DataAccess;
            recaller.addTimeEntry(time);

            return await recaller.ExecuteAction();
        }

        #endregion

        #region CRUD Operations
        /// <summary>
        /// Save selected Time, if Id is null it will create a new one, 
        /// otherwise it will update the selected Time.
        /// </summary>
        /// <returns></returns>
        public override async Tasks.Task<bool> ForcedSave()
        {
            bool result = false;
            msdyn_timeentry time = this.Time;

            if (time != null)
            {
                BookableResource bookableResource = await TimeHelper.GetBookableResourceForUser(await this.DataAccess.GetLoggedUserId(true));

                if (TimeHelper.isBookableResourceValid(bookableResource))
                {
                    time.msdyn_bookableresource = new EntityReference(BookableResource.EntityLogicalName, bookableResource.BookableResourceId.GetValueOrDefault());

                    // If Time can't be saved, don't allow the client to save, 
                    // return false so user can make the corresponding corrections.
                    if (this.CanTimeBeSaved(time))
                    {
                        // If Time doesn't have an Id, it need to be created
                        if (time.Id == null || time.Id == Guid.Empty)
                        {
                            // Always clear the time entry id when creating a new entry
                            // because CRM validation treats Guid.Empty as an invalid id for a new record.
                            Guid? timeEntryId = await this.DataAccess.Create(time);

                            if (timeEntryId != null && timeEntryId != Guid.Empty)
                            {
                                time.Id = (Guid)timeEntryId;
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = await this.DataAccess.Update(time);
                        }

                        // Update status of Time
                        if (this.HasPendingDataToSave)
                        {
                            this.HasPendingDataToSave = !result;
                        }

                        // If the validation method hasn't thrown an error message but save failed, throw an error message here.
                        if (!result)
                        {
                            await MessageCenter.ShowErrorMessage(AppResources.SaveError);
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// If the Time is not yet created, just delete from memory. otherwise delete from server
        /// </summary>
        /// <param name="omitWarningMessage">True if deletion would happen without a confirmation from the user</param>
        /// <returns>True if the Time was deleted</returns>
        public async Tasks.Task<bool> Delete(bool omitWarningMessage = false)
        {
            msdyn_timeentry time = this.Time;
            if (time != null)
            {
                if (time.Id != Guid.Empty && this.CanEdit())
                {
                    if (omitWarningMessage || await MessageCenter.ShowDialog(AppResources.DeleteWarningTime, null, null))
                    {
                        return await this.DataAccess.Delete(time.LogicalName, time.Id);
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
        /// Validates if time entry can be saved.
        /// </summary>
        /// <param name="Time">The time entry for which to validate.</param>
        /// <returns>True if the time entry can be saved, false otherwise.</returns>
        public virtual bool CanTimeBeSaved(msdyn_timeentry timeEntry)
        {
            // Check that the date field is filled.
            if (timeEntry.msdyn_date == null)
            {
                MessageCenter.ShowErrorMessage(AppResources.DateInvalidWarning).DoNotAwait();
                return false;
            }

            // Check that the total duration (including this new time entry) does not exceed 24 hours for a particular day.
            Dictionary<DateTime, int> durationsPerDay = TimeCollectionViewModel.DurationPerDay;
            DateTime date = (DateTime)timeEntry.msdyn_date;  // Use msdyn_date since that is the one stored in the entity.

            if (durationsPerDay.ContainsKey(date))
            {
                // Subtract the selected time entry saved duration for update scenario to prevent double adding the total durations.
                if (this.Time.msdyn_timeentryId != null || this.Time.msdyn_timeentryId != Guid.Empty)
                {
                    durationsPerDay[date] -= (int)this.Time.msdyn_duration;
                }

                if ((durationsPerDay[date] + timeEntry.msdyn_duration) > (24 * 60))
                {
                    MessageCenter.ShowErrorMessage(AppResources.TimeExceeded).DoNotAwait();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The time entry can only be edited if its not in submitted or approved status.
        /// </summary>
        /// <returns></returns>
        public bool CanEdit()
        {
            if (this.Time == null || this.Time.msdyn_entryStatus == null)
            {
                return true;
            }
            msdyn_timeentry_msdyn_entrystatus status = (msdyn_timeentry_msdyn_entrystatus)Enum.ToObject(typeof(msdyn_timeentry_msdyn_entrystatus), this.Time.msdyn_entryStatus.Value);
            return (!((status == msdyn_timeentry_msdyn_entrystatus.Submitted) || (status == msdyn_timeentry_msdyn_entrystatus.Approved)));
        }

        /// <summary>
        /// Validates if a time entry is submitted.
        /// </summary>
        /// <returns>True if it is submitted, false otherwise.</returns>
        public bool IsSubmitted()
        {
            bool result = false;

            if (this.Time == null || this.Time.msdyn_entryStatus == null)
            {
                return false;
            }

            if ((msdyn_timeentry_msdyn_entrystatus)Enum.ToObject(typeof(msdyn_timeentry_msdyn_entrystatus), this.Time.msdyn_entryStatus.Value) == msdyn_timeentry_msdyn_entrystatus.Submitted)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region OnSelectedMethods
        /// <summary>
        /// Search in the ReferenceData dictionary the object for given type and key
        /// and added to selected Time
        /// </summary>
        /// <param name="key">selected item from UI</param>
        public bool OnProjectSelected(string key, int index)
        {
            msdyn_project selectedProject = this.GetObjectByName<msdyn_project>(key);
            if (selectedProject != null)
            {
                if (Time.msdyn_project == null || selectedProject.Id != Time.msdyn_project.Id)
                {
                    EntityReference newReference = new EntityReference(msdyn_project.EntityLogicalName, selectedProject.Id);
                    Time.msdyn_project = newReference;
                }
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Load data for projects
        /// </summary>
        /// <returns></returns>
        public override async Tasks.Task LoadReferenceData()
        {
            this.IsBusy = true;
            this.DataAccess.AccessMode = DataAccessMode.SyncConnection;

            QueryExpression queryExpression = new QueryExpression(msdyn_project.EntityLogicalName);
            queryExpression.ColumnSet = new ColumnSet("msdyn_projectid", "msdyn_subject", "msdyn_description");

            // Retrieve projects based on project team
            LinkEntity projectTeamLink = queryExpression.AddLink(msdyn_projectteam.EntityLogicalName, "msdyn_projectid", msdyn_project.EntityLogicalName, JoinOperator.Inner);
            projectTeamLink.Columns.AddColumn("msdyn_bookableresourceid");
            projectTeamLink.EntityAlias = "msdyn_projectteam";

            Guid userId = await this.DataAccess.GetLoggedUserId(true);
            BookableResource bookableResource = await TimeHelper.GetBookableResourceForUser(userId);
            bool searchAllProjects = true;
            ProjectFilters = new FilterQuery[2];

            if (userId != Guid.Empty && TimeHelper.isBookableResourceValid(bookableResource))
            {
                ConditionExpression crmUserExpression = new ConditionExpression(msdyn_projectteam.EntityLogicalName, "msdyn_bookableresourceid", ConditionOperator.Equal, bookableResource.BookableResourceId.GetValueOrDefault());
                queryExpression.Criteria = new FilterExpression();
                queryExpression.Criteria.AddCondition(crmUserExpression);
                searchAllProjects = !await this.LoadReferenceData<msdyn_project>(queryExpression);
                currentProjectFilter = 0;
                ProjectFilters[currentProjectFilter] = new FilterQuery(AppResources.ShowMyProjects, queryExpression, this.GetReferenceKeys<msdyn_project>());
            }

            if (searchAllProjects)
            {
                // If my projects didn't return values or I don't have the current user id, query all projects.
                queryExpression.Criteria = null;
                await this.LoadReferenceData<msdyn_project>(queryExpression);
                currentProjectFilter = 1;
                ProjectFilters[currentProjectFilter] = new FilterQuery(AppResources.ShowAllProjects, queryExpression, this.GetReferenceKeys<msdyn_project>());
            }
            else
            {
                // Else, store the all projects query for use when the user wants to see all projects.
                queryExpression.Criteria = null;
                ProjectFilters[AllProjectsFilterIndex] = new FilterQuery(AppResources.ShowAllProjects, queryExpression, null);
            }

            this.IsBusy = false;
        }

        /// <summary>
        /// Return the keys of the reference data ensuring current data in model is present
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ICollection<string> GetReferenceKeys<TEntity>(string attributeLogicalName)
            where TEntity : Entity, new()
        {
            if (attributeLogicalName != null)
            {
                EntityReference currentReference = this.Time.GetAttributeValue<EntityReference>(attributeLogicalName);
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
        /// Return the keys of the reference data that is gotten for the specified project
        /// </summary>
        /// <typeparam name="TEntity">A project entity</typeparam>
        /// <returns></returns>
        public async Tasks.Task<ICollection<string>> GetTasksForProject(EntityReference project)
        {
            this.IsBusy = true;

            QueryExpression queryExpression = new QueryExpression(msdyn_projecttask.EntityLogicalName);
            queryExpression.ColumnSet = new ColumnSet("msdyn_subject", msdyn_project.EntityLogicalName);

            //Retrieve projects based on project
            ConditionExpression projectExpression = new ConditionExpression(msdyn_projecttask.EntityLogicalName, msdyn_project.EntityLogicalName, ConditionOperator.Equal, project.Id);
            queryExpression.Criteria = new FilterExpression();
            queryExpression.Criteria.AddCondition(projectExpression);

            this.DataAccess.AccessMode = DataAccessMode.SyncConnection;
            await this.LoadReferenceData<msdyn_projecttask>(queryExpression);

            this.IsBusy = false;
            return this.GetReferenceKeys<msdyn_projecttask>();
        }

        public TOptionSet[] GetAllOptionSetValues<TOptionSet>()
        {
            List<string> stringValues = new List<string>();
            TOptionSet[] allValues = (TOptionSet[])Enum.GetValues(typeof(TOptionSet));
            return allValues;
        }

        /// <summary>
        /// It will rotate between all the available filters of project and return the next one.
        /// If the limit is reached it will started from the beginning
        /// </summary>
        /// <returns></returns>
        public async Tasks.Task<FilterQuery> GetFilterProject(bool switchFilter = false)
        {
            FilterQuery filter = null;
            if(ProjectFilters != null)
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
                if(filter != null && filter.Result == null
                    && await this.LoadReferenceData<msdyn_project>(filter.QueryExpression))
                {
                    filter.Result = this.GetReferenceKeys<msdyn_project>();
                }
            }
            return filter;
        }
    }
}
