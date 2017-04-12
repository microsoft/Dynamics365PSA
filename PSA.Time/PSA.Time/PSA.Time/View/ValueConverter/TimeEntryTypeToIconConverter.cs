using Common.Model;
using Microsoft.Xrm.Sdk.Samples;
using System;
using Xamarin.Forms;

namespace PSA.Time.View.ValueConverter
{
    public class TimeEntryTypeToIconConverter : IValueConverter
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
            OptionSetValue optionSetValue = value as OptionSetValue;
            if (optionSetValue != null)
            {
                msdyn_timeentrytype type = (msdyn_timeentrytype)Enum.ToObject(typeof(msdyn_timeentrytype), optionSetValue.Value);
                switch (type)
                {
                    case msdyn_timeentrytype.Work:
                        return "\uE555";
                    case msdyn_timeentrytype.Vacation:
                        return "\uE700";
                    case msdyn_timeentrytype.Absence:
                        return "\uE705";
                }
            }
            return "\uE555";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
