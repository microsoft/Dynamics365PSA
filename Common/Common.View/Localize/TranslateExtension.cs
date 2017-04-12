using Common.Utilities.Resources;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Common.View.Localize
{
    /// <summary>
    /// This class fetches a string from the AppResource file and makes it available for use in the Text attribute of a XAML file
    /// </summary>
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        readonly CultureInfo ci;
        const string ResourceId = "Common.Utilities.Resources.AppResources";

        public TranslateExtension()
        {
            ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            ResourceManager resmgr = new ResourceManager(ResourceId, typeof(AppResources).GetTypeInfo().Assembly);

            var translation = resmgr.GetString(Text, ci);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, ci.Name), "Text");
#else
                translation = Text; 
#endif
            }

            //TODO remove in next iteration when label changes are allowed 
            if (Text == "Justification")
                translation = string.Format(translation.Replace(@"\n", string.Empty), string.Empty);

            return translation;
        }
    }
}
