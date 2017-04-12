using System.Globalization;

namespace Common.View.Localize
{
    /// <summary>
    /// Only implement if you need to use localization in XAML
    /// </summary>
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale();
    }
}
