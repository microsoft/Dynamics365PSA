using Common.Utilities.Resources;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Common.Windows.Utilities
{
    public class WindowsDialog
    {
        private static WindowsDialog current;
        public static WindowsDialog Current
        {
            get
            {
                if (current == null)
                {
                    current = new WindowsDialog();
                }
                return current;
            }
        }

        private WindowsDialog() { }

        public async Task ShowMessageAction(string message, string title = null, string buttonOk = null)
        {
            buttonOk = buttonOk ?? AppResources.OK;

            var messageDialog = new MessageDialog(message, title);
            messageDialog.Commands.Add(new UICommand(buttonOk));
            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();
        }

        public async Task<bool> ShowDialogAction(string message, string title = null, string buttonOk = null, string buttonCancel = null)
        {
            buttonOk = buttonOk ?? AppResources.OK;
            buttonCancel = buttonCancel ?? AppResources.Cancel;

            var messageDialog = new MessageDialog(message, title);
            messageDialog.Commands.Add(new UICommand(buttonOk));
            messageDialog.Commands.Add(new UICommand(buttonCancel));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            var messageResult = await messageDialog.ShowAsync();
            return messageResult.Label == buttonOk;
        }
    }
}
