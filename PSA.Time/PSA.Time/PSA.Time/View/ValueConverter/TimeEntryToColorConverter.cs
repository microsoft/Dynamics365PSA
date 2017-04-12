using Common.Model;
using System;
using Xamarin.Forms;

namespace PSA.Time.View.ValueConverter
{
    public class TimeEntryToColorConverter : IValueConverter
    {
        /// <summary>
        /// Returns the icon given the entry type
        /// </summary>
        /// <param name="value">msdyn_timeentrytype</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Icon based on expense type</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color result = Color.FromHex("7F7F7F");

            if (value == null)
            {
                return result;
            }

            msdyn_timeentry time = value as msdyn_timeentry;

            if ((msdyn_timeentrytype)time.msdyn_type.Value == msdyn_timeentrytype.Work 
                && time.msdyn_project != null 
                && time.msdyn_project.Id != null)
            {
                result = GetColor(GetIndexWithProjectGUID(time.msdyn_project.Id.Value, culture.NumberFormat));
            }

            return result;
        }

        private int GetIndexWithProjectGUID(Guid guid, IFormatProvider formatProvider)
        {
            int numberOfColors = 12;
            int charsToAdd = 8;
            int totalValue = 0;
            int i = 0;
            char[] guidArray = guid.ToString().ToCharArray();

            // get the sum of a number of hex characters of the guid. The max sum should be a multiple of the number
            // of available colors (twelve). Choosing 8 characters 8 x 15 = 120.
            while (i < guidArray.Length && charsToAdd > 0)
            {
                int value;
                if (int.TryParse(guidArray[i].ToString(), System.Globalization.NumberStyles.HexNumber, formatProvider, out value))
                {
                    charsToAdd--;
                    totalValue += value;
                }
                i++;
            }

            
            return totalValue % numberOfColors;
        }

        private Color GetColor(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0:
                    // blue
                    return Color.FromHex("0082bf");
                case 1:
                    // red
                    return Color.FromHex("fc483f");
                case 2:
                    // green
                    return Color.FromHex("22b14c");
                case 3:
                    // orange
                    return Color.FromHex("ff7f27");
                case 4:
                    // yellow
                    return Color.FromHex("ffc90e");
                case 5:
                    // purple
                    return Color.FromHex("a349a4");
                case 6:
                    // dark blue
                    return Color.FromHex("005E8A");
                case 7:
                    // dark red
                    return Color.FromHex("D10E03");
                case 8:
                    // dark green
                    return Color.FromHex("177B35");
                case 9:
                    // dark orange
                    return Color.FromHex("C85000");
                case 10:
                    // dark yellow
                    return Color.FromHex("CA9600");
                case 11:
                    // dark purple
                    return Color.FromHex("6C326C");
                default:
                    // light blue / teal
                    return Color.FromHex("43B7CB");

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
