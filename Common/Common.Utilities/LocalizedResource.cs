using Common.Utilities.Resources;

namespace Common.Utilities
{
    /// <summary>
    /// Class that make AppResources available to native WinPhone apps. Has to be in this project because AppResources has an internal constructor
    /// </summary>
    public class LocalizedResource
    {
        private static AppResources _localizedResources = new AppResources();
        public AppResources LocalizedResources { get { return _localizedResources; } }

        //TODO remove in next iteration when label changes are allowed 
        public static string GetCorrectedJustificationText(string replaceText = "") // Can't set to string.empty because that's not a compile time constant 
        {
            return string.Format(AppResources.Justification.Replace(@"\n", string.Empty), replaceText);
        }
    }
}
