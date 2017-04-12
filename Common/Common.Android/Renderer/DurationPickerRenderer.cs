using Android.App;
using Android.Runtime;
using Android.Widget;
using Common.Android.Renderer;
using Common.View.CustomControl;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DurationPickerEx), typeof(DurationPickerRenderer))]

namespace Common.Android.Renderer
{
    /// <summary>
    /// Renders a TimePicker without the AM/PM selector.
    /// </summary>
    public class DurationPickerRenderer : ViewRenderer<Xamarin.Forms.TimePicker, EditText>, TimePickerDialog.IOnTimeSetListener, IJavaObject, IDisposable
    {
        private TimePickerDialog dialog = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);
            this.SetNativeControl(new global::Android.Widget.EditText(Forms.Context));
            TimeSpan time = (TimeSpan)this.Element.GetValue(Xamarin.Forms.TimePicker.TimeProperty);
            this.Control.Text = time.ToString(@"hh\:mm");
            this.Control.Click += Control_Click;
            this.Control.FocusChange += Control_FocusChange;
        }

        void Control_FocusChange(object sender, global::Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ShowTimePicker();
            }
        }

        void Control_Click(object sender, EventArgs e)
        {
            ShowTimePicker();
        }

        private void ShowTimePicker()
        {
            if (dialog == null)
            {
                TimeSpan time = (TimeSpan)this.Element.GetValue(Xamarin.Forms.TimePicker.TimeProperty);

                dialog = new TimePickerDialog(Forms.Context, this, time.Hours, time.Minutes, true);
            }

            dialog.Show();
        }

        public void OnTimeSet(global::Android.Widget.TimePicker view, int hourOfDay, int minute)
        {
            var time = new TimeSpan(hourOfDay, minute, 0);
            this.Element.SetValue(Xamarin.Forms.TimePicker.TimeProperty, time);

            this.Control.Text = time.ToString(@"hh\:mm");
        }
    }

}