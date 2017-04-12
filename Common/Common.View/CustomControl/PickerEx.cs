using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace Common.View.CustomControl
{
    public class PickerEx : Picker
    {
        public PickerEx()
        {
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public static BindableProperty ItemsSourceProperty =  BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PickerEx), null, propertyChanged: OnItemsSourceChanged);
        public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(PickerEx), null, propertyChanged: OnSelectedItemChanged);

        public delegate void ChangedEventHandler(object sender, object oldvalue, object newvalue);
        public event ChangedEventHandler SelectedItemChanged;

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = bindable as PickerEx;

            // capture the current selected item for use after updating.
            var selectedItem = picker.SelectedItem;

            picker.Items.Clear();
            if (newValue != null)
            {
                IEnumerable listNewValue = (IEnumerable)newValue;
                foreach (var item in listNewValue)
                {
                    picker.Items.Add(item.ToString());
                }
                OnSelectedItemChanged(bindable, null, selectedItem);
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                SelectedItem = "";
            }
            else
            {
                SelectedItem = Items[SelectedIndex];
            }
            this.InvalidateMeasure();
        }

        public static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as PickerEx;
            if (newvalue != null && picker.Items.Contains(newvalue))
            {
                picker.SelectedIndex = picker.Items.IndexOf(newvalue.ToString());
            }
            else
            {
                picker.SelectedIndex = -1;
            }
            picker.SelectedItemChanged?.Invoke(bindable, oldvalue, newvalue);
        }
    }
}
