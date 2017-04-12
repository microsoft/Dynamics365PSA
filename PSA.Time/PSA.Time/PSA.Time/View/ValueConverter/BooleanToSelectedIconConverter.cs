using System;
using System.Globalization;
using Xamarin.Forms;

namespace PSA.Time.View
{
    /// <summary>
    /// Convert bool value to checkbox icon from Dynamics Symbol font.
    /// </summary>
    class BooleanToSelectedIconConverter : IValueConverter
    {
        private const string DynamicsSymbolUnchecked = "\uE003";
        private const string DynamicsSymbolChecked = "\uE0A2";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool selected = false;

            if (value != null)
            {
                selected = (bool)value;
            }

            return selected ? DynamicsSymbolChecked : DynamicsSymbolUnchecked;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
