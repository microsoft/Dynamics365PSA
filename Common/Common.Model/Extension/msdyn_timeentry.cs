using Microsoft.Xrm.Sdk.Samples;
using System;

namespace Common.Model
{
    /// <summary>
    /// Main entity to hold time entry information.
    /// </summary>
    public partial class msdyn_timeentry : Entity, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// The maximum length for the description fields.
        /// </summary>
        public const int DescriptionLength = 100;

        /// <summary>
        /// CRM does an automatic conversion of the date fields to local time. In order to count for that, we need the following:
        /// - When reading from the entity, we can read as is since we expect all clients to correct all date times for time zone conversions.
        /// - When writing to the entity, convert the time portion of the date time field to 12pm to minimize likelihood of date change after conversion. 
        /// </summary>
        public DateTime? msdyn_date_utc
        {
            get
            {
                return msdyn_date;
            }
            set
            {
                if (value == null)
                {
                    msdyn_date = null;
                    return;
                }

                // Ensure that the date time is stored in UTC 12:00.
                DateTime utcDateTime = value.Value;
                msdyn_date = new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, 12, 0, 0, DateTimeKind.Utc);
            }
        }

        public msdyn_timeentry_msdyn_entrystatus? EntryStatus
        {
            get
            {
                OptionSetValue value = this.msdyn_entryStatus;
                if (value == null)
                {
                    return null;
                }
                else
                {
                    return (msdyn_timeentry_msdyn_entrystatus)value.Value;
                }
            }
            set
            {
                if (value == null)
                {
                    this.msdyn_entryStatus = null;
                }
                else
                {
                    this.msdyn_entryStatus = new OptionSetValue((int)value);
                }
            }
        }

        private bool selected = false;
        /// <summary>
        /// Indicates if a time entry is selected. Supports multiselect for actions on multiple time entries.
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value != selected)
                {
                    selected = value;
                    this.OnPropertyChanged("Selected");
                }
            }
        }
    }
}
