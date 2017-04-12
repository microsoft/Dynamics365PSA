using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace Common.Windows.Utilities
{
    public static class StatusBar
    {
        public static async Task ChangeStatusBarColorAsync(string resourceKey)
        {
            var color = (Color)(Application.Current.Resources[resourceKey]);
            await ChangeStatusBarColorAsync(color);
        }

        public static async Task ChangeStatusBarColorAsync(Color color)
        {
            var statusBar = global::Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = color;
            statusBar.BackgroundOpacity = 1;
            await statusBar.ShowAsync();
        }
    }
}
