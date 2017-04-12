using Common.View.CustomControl;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DurationPickerEx), typeof(Common.iOS.Renderer.DurationPickerRenderer))]
namespace Common.iOS.Renderer
{
    public class DurationPickerRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);
            var native = (UITextField)Control;
            DurationPickerEx newElement = e.NewElement as DurationPickerEx;

            if (native != null && newElement != null)
            {
                var timePicker = (UIDatePicker)Control.InputView;
                timePicker.Locale = new NSLocale("no_nb");
            }
        }
    }
}