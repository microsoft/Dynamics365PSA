using System;
using System.IO;
using Xamarin.Forms;

namespace Common.View.ValueConverter
{
    public class StringToImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string  to ImageSource
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource result = null;
            string documentBody = value as string;
            if (documentBody != null)
            {
                result = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(documentBody)));               
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
