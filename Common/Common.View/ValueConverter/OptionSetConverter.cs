using Common.Utilities.Metadata;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.View.ValueConverter
{
    public class OptionSetConverter<TOptionSet> : IValueConverter
        where TOptionSet : struct
    {
        /// <summary>
        /// Convert from an Option set to localized string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OptionSetValue optionSetValue = value as OptionSetValue;
            if (optionSetValue != null)
            {
                TOptionSet currentValue = (TOptionSet)Enum.ToObject(typeof(TOptionSet), optionSetValue.Value);
                // return LabelHandler.GetLabel(currentValue.ToString()).ToUpper();
                return currentValue.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new OptionSetValue((int)Enum.Parse(typeof(TOptionSet), value.ToString()));
        }
    }
}
