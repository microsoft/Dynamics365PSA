using Common.Utilities.Resources;
using System;
using Xamarin.Forms;

namespace Common.View.ValueConverter
{
    public class DurationToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a duration (in minutes) to localized string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>A localized string with the duration in hours and minutes (ex. 02:30)</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int?)
            {
                int? intValue = (int?)value;

                if (intValue.HasValue)
                {                    
                    TimeSpan time = TimeSpan.FromMinutes(intValue.Value);

                    if (time.Minutes != 0)
                    {
                        return string.Format(AppResources.DurationHoursMinute, time.Hours, time.Minutes);                        
                    }
                    else
                    {
                        // Use time.TotalHours instead of time.Hours to support showing 24h.
                        return string.Format(AppResources.DurationHours, time.TotalHours);
                    }                    
                }
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
