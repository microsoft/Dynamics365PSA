using Xamarin.Forms;

namespace Common.View.CustomControl
{
    public class ListViewEx : ListView
    {
        public static readonly BindableProperty ItemTemplateClassNameProperty =
          BindableProperty.Create<ListViewEx, string>(p => p.ItemTemplateClassName, null);

        public string ItemTemplateClassName
        {
            get { return (string)GetValue(ItemTemplateClassNameProperty); }
            set { SetValue(ItemTemplateClassNameProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateAssemblyNameProperty =
          BindableProperty.Create<ListViewEx, string>(p => p.ItemTemplateAssemblyName, null);

        public string ItemTemplateAssemblyName
        {
            get { return (string)GetValue(ItemTemplateAssemblyNameProperty); }
            set { SetValue(ItemTemplateAssemblyNameProperty, value); }
        }
    }
}
