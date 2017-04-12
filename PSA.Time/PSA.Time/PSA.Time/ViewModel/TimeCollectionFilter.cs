using System;
using System.ComponentModel;

namespace PSA.Time.ViewModel
{
    /// <summary>
    /// Help with managing the date filter used for the time entry list.
    /// </summary>
    public class TimeCollectionDateFilter : INotifyPropertyChanged
    {
        private DateTime startDate;
        private DateTime endDate;
        private TimeSpan periodLength;

        public event PropertyChangedEventHandler PropertyChanged;

        private const string StartDatePropertyName = "StartDate";
        private const string EndDatePropertyName = "EndDate";
        private const string FilterTextPropertyName = "FilterText";

        /// <summary>
        /// Start date for the current list.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
                // Update end
                endDate = startDate + periodLength - TimeSpan.FromDays(1);

                this.OnPropertyChanged();                
            }
        }

        /// <summary>
        /// End date.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                DateTime dateOnlyValue = value.Date;

                if (dateOnlyValue < startDate)
                {
                    throw new ArgumentOutOfRangeException("Cannot set end date before the start date");
                }

                endDate = dateOnlyValue;
                this.setPeriodFromDates();

                this.OnPropertyChanged();
            }
        }        

        /// <summary>
        /// TimeSpan representing the length of the filter.
        /// </summary>
        public TimeSpan Period
        {
            get
            {
                return periodLength;
            }
        }

        /// <summary>
        /// The string representing the current date filter range.
        /// </summary>
        public string FilterText
        {
            get
            {
                return this.ToString();
            }
        }

        public TimeCollectionDateFilter()
        {
            // set default
            DateTime today = DateTime.Today;
            startDate = today - TimeSpan.FromDays((int)today.DayOfWeek);
            endDate = startDate + TimeSpan.FromDays(6);

            this.setPeriodFromDates();
        }

        public TimeCollectionDateFilter(DateTime start, DateTime end)
        {
            startDate = start.Date;
            endDate = end.Date;
            this.setPeriodFromDates();
        }

        /// <summary>
        /// Update the period from current start and end dates.
        /// </summary>
        private void setPeriodFromDates()
        {
            // Need to add 1 day so the period is the correct length.
            periodLength = endDate - startDate + TimeSpan.FromDays(1);
        }

        public override string ToString()
        {
            // format "d" is short date
            return string.Format("{0:d}-{1:d}", startDate, endDate);
        }

        /// <summary>
        /// Move to the next period.
        /// </summary>
        public void Increment()
        {
            // Increasing the start date property will push end date forward as well
            this.StartDate += this.periodLength;            
        }

        /// <summary>
        /// Move to the previous period.
        /// </summary>
        public void Decrement()
        {
            this.startDate -= this.periodLength;
            this.endDate -= this.periodLength;

            this.OnPropertyChanged(StartDatePropertyName);
            this.OnPropertyChanged(EndDatePropertyName);
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string propertyName = "")
        {
            if (PropertyChanged != null)
            {               
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

                if (propertyName == StartDatePropertyName || propertyName ==  EndDatePropertyName)
                {
                    this.OnPropertyChanged(FilterTextPropertyName);
                }
            }
        }
    }
}
