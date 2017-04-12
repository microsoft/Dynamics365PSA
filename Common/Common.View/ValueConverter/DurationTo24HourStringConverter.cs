using System;
using Xamarin.Forms;

namespace Common.View.ValueConverter
{
    public class DurationTo24HourStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a duration (in minutes) to a string in the format HH:mm.
        /// </summary>
        /// <param name="value">The duration value from the server.</param>
        /// <param name="targetType">The data type expected from the conversion.</param>
        /// <param name="parameter">A parameter to the convertion, null in this case.</param>
        /// <param name="culture">Provides information about a specific culture.</param>
        /// <returns>A string object converted to the correct format if the input is valid, empty string otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int?)
            {
                int? intValue = (int?)value;
                if (intValue.HasValue)
                {
                    TimeSpan finalTimeSpan = TimeSpan.FromMinutes(intValue.Value);
                    // Use finalTimeSpan.TotalHours instead of finalTimeSpan.Hours to support showing 24:00.
                    return string.Format("{0:00}:{1:00}", finalTimeSpan.TotalHours, finalTimeSpan.Minutes);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Not implemented since its a one-way binding.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
