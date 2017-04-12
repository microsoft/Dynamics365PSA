using Xamarin.Forms;

namespace Common.View.CustomControl
{
    /// <summary>
    /// Custom TimePicker that has no AM/PM selector (done with custom renderers)
    /// </summary>
    public class DurationPickerEx : TimePicker
    {
        public DurationPickerEx() : base() {
            this.Format = @"hh\:mm";
        }
    }
}
