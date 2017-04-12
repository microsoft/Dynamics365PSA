using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PSA.Time.View.ValueConverter
{
    public class TimeEntryToTitleConverter : IValueConverter
    {
        /// <summary>
        /// Returns the title string given the time entry
        /// </summary>
        /// <param name="value">msdyn_timeentrytype</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>title string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            msdyn_timeentry time = value as msdyn_timeentry;

            if (time == null)
            {
                return string.Empty;
            }
            else if (time.msdyn_project != null && time.msdyn_project.Name != null)
            {
                return time.msdyn_project.Name;
            }
            else
            {
                msdyn_timeentrytype currentValue = (msdyn_timeentrytype)Enum.ToObject(typeof(msdyn_timeentrytype), time.msdyn_type.Value);
                return currentValue.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
