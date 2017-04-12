using Common.Model;
using Microsoft.Xrm.Sdk.Samples;
using System;
using Xamarin.Forms;

namespace PSA.Expense.View.ValueConverter
{
    public class ExpenseTypeToIconConverter : IValueConverter
    {
        /// <summary>
        /// Returns the icon given the expense type
        /// </summary>
        /// <param name="value">msdyn_expensetypes</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Icon based on expense type</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OptionSetValue optionSetValue = value as OptionSetValue;
            string icon = "\uE539";
            if (optionSetValue != null)
            {
                msdyn_expensetypes expenseType = (msdyn_expensetypes)Enum.ToObject(typeof(msdyn_expensetypes), optionSetValue.Value);

                switch (expenseType)
                {
                    case msdyn_expensetypes.Airline:
                        icon = "\uE700";
                        break;
                    case msdyn_expensetypes.CarRental:
                        icon = "\uE701";
                        break;
                    case msdyn_expensetypes.Gift:
                        icon = "\uE704";
                        break;
                    case msdyn_expensetypes.Hotel:
                        icon = "\uE705";
                        break;
                    case msdyn_expensetypes.Meal:
                        icon = "\uE706";
                        break;
                    case msdyn_expensetypes.Conference:
                        icon = "\uE702";
                        break;
                    case msdyn_expensetypes.Entertainment:
                        icon = "\uE703";
                        break;
                    case msdyn_expensetypes.Mileage:
                        icon = "\uE707";
                        break;
                    case msdyn_expensetypes.Miscellanous:
                    case msdyn_expensetypes.Perdiem:
                    default:
                        icon = "\uE539";
                        break;
                };
            }
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
