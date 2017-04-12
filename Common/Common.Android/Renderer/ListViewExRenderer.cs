using Android.Widget;
using Common.View.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListViewEx), typeof(Common.Android.Renderer.ListViewExRenderer))]
namespace Common.Android.Renderer
{
    public class ListViewExRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            global::Android.Widget.ListView native = Control as global::Android.Widget.ListView;
            ListViewEx newElement = e.NewElement as ListViewEx;

            if (native != null && newElement != null)
            {
                SetAdapter(native, newElement);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            global::Android.Widget.ListView native = Control as global::Android.Widget.ListView;
            ListViewEx newElement = sender as ListViewEx;

            if (native != null && newElement != null)
            {
                if (e.PropertyName == "ItemsSource")
                {
                    SetAdapter(native, newElement);
                }
            }
        }

        private void SetAdapter(global::Android.Widget.ListView native, ListViewEx newElement)
        {
            var adapterClassName = newElement.ItemTemplateClassName;
            var adapterAssemblyName = newElement.ItemTemplateAssemblyName;

            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(adapterAssemblyName)).ToList();
            Assembly assembly = assemblies.FirstOrDefault(a => a.GetType(adapterClassName, false) != null);

            if (assembly == null)
                throw new Exception("Invalid ItemTemplateAssemblyName");

            var adapterType = assembly.GetType(adapterClassName);

            if (adapterType == null)
                throw new Exception("Invalid ItemTemplateClassName");

            if (adapterType.IsSubclassOf(typeof(IListAdapter)))
                throw new Exception("ItemTemplateClassName must be a sub-class of ArrayAdapter");

            try
            {
                var adapter = (IListAdapter)Activator.CreateInstance(adapterType, Context, newElement.ItemsSource);
                native.Adapter = adapter;
            }
            catch (TargetInvocationException)
            {
                throw new Exception("The adapter defined in ItemTemplateClassName must have a constructor with (Context context, IList<T> items) and it must handle when IList<T> items == null");
            }
            catch (MissingMethodException)
            {
                throw new Exception("The adapter defined in ItemTemplateClassName must have a constructor with (Context context, IList<T> items) and it must handle when IList<T> items == null");
            }
        }
    }
}