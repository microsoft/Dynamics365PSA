using Common.Model;
using System;
using System.Collections.ObjectModel;

namespace PSA.Time.Model
{
    /// <summary>
    /// Class to represent a logic group of Times. e.g: Times in draft status for company X.
    /// </summary>
    public class TimeCollection : ObservableCollection<msdyn_timeentry>, IComparable<DateTime>
    {
        public DateTime Date { private set; get; }

        public TimeCollection(DateTime date)
        {
            this.Date = date;
        }

        /// <summary>
        /// Return the name of the grouping.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Date.ToString("MM-dd-yyyy");
        }

        public int CompareTo(DateTime dt)
        {
            return this.Date.CompareTo(dt);
        }
    }
}