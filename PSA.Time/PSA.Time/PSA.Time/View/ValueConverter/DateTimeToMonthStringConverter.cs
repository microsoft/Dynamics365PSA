using System;
using Xamarin.Forms;

namespace PSA.Time.View.ValueConverter
{
    /// <summary>
    /// This class handles all conversions from DateTime object to a string (September 2015 for instance) and vice versa.
    /// </summary>
    public class DateTimeToMonthStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a DateTime object to a user friendly month and year string.
        /// </summary>
        /// <param name="value">The DateTime value from the model.</param>
        /// <param name="targetType">The data type expected from the conversion, DateTime in this case.</param>
        /// <param name="parameter">A parameter to the convertion, null in this case.</param>
        /// <param name="culture">Provides information about a specific culture.</param>
        /// <returns>A string object converted to the correct format if the input is valid, empty string otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;                
                return dateTime.ToString("MMMM yyyy");
            }
            return String.Empty;
        }

        /// <summary>
        /// Convert from a string of month and year to a DateTime object.
        /// </summary>
        /// <param name="value">The string value from the control.</param>
        /// <param name="targetType">The data type expected from the conversion, string in this case.</param>
        /// <param name="parameter">A parameter to the convertion, null in this case.</param>
        /// <param name="culture">Provides information about a specific culture.</param>
        /// <returns>A string object converted to the correct format if the input is valid, empty string otherwise.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is String && !String.IsNullOrEmpty(value.ToString()))
            {                
                return System.Convert.ToDateTime(value);
            }
            return DateTime.Now;
        }
    }
}
