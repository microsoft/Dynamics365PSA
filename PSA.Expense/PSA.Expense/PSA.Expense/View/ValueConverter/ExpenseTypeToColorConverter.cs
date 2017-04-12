using Common.Model;
using Common.View;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PSA.Expense.View.ValueConverter
{
    class ExpenseTypeToColorConverter : IValueConverter
    {
        /// <summary>
        /// Return the color based on expense type
        /// </summary>
        /// <param name="value">msdyn_expensetypes</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>color based on expense type</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OptionSetValue optionSetValue = value as OptionSetValue;
            Color color = Color.FromHex("D10E03");
            if (optionSetValue != null)
            {
                msdyn_expensetypes expenseType = (msdyn_expensetypes)Enum.ToObject(typeof(msdyn_expensetypes), optionSetValue.Value);

                switch (expenseType)
                {
                    case msdyn_expensetypes.Airline:
                        color = Color.FromHex("ff7f27");
                        break;
                    case msdyn_expensetypes.CarRental:
                        color = Color.FromHex("fc483f");
                        break;
                    case msdyn_expensetypes.Gift:
                        color = Color.FromHex("a349a4");
                        break;
                    case msdyn_expensetypes.Hotel:
                        color = Color.FromHex("0082bf");
                        break;
                    case msdyn_expensetypes.Meal:
                        color = Color.FromHex("22b14c");
                        break;
                    case msdyn_expensetypes.Conference:
                        color = Color.FromHex("005E8A");
                        break;
                    case msdyn_expensetypes.Entertainment:
                        color = Color.FromHex("ffc90e");
                        break;
                    case msdyn_expensetypes.Mileage:
                        color = Color.FromHex("177B35");
                        break;
                    case msdyn_expensetypes.Miscellanous:
                        color = Color.FromHex("D10E03");
                        break;
                    case msdyn_expensetypes.Perdiem:
                    default:
                        color = Color.FromHex("C85000");
                        break;
                };
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

