using Common.Utilities.Metadata;
using Common.Utilities.Resources;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class MessageCenter
    {
        public delegate Task ShowMessageAction(string message, string title = null, string buttonOk = null);
        public delegate Task<bool> ShowDialogAction(string message, string title = null, string buttonOk = null, string buttonCancel = null);
        public static ShowMessageAction ShowErrorDelegate;
        public static ShowMessageAction ShowMessageDelegate;
        public static ShowDialogAction ShowDialogDelegate;

        /// <summary>
        /// Show an error to the user, if the delegate to show the error is not set it will try to display the error as a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static async Task ShowError(string message, string title = null)
        {
            title = title ?? AppResources.errorTitle;
            message = CorrectForReturnChar(message);
            if (ShowErrorDelegate != null)
            {
                await ShowErrorDelegate(message, title);
            }
            else
            {
                await ShowMessageDelegate(message, title);
            }
        }

        /// <summary>
        /// Show a message to the user, if the delegate is not set the message will get not displayed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static async Task ShowMessage(string message, string title = null)
        {
            message = CorrectForReturnChar(message);
            if (ShowMessageDelegate != null)
            {
                await ShowMessageDelegate(message, title);
            }
#if DEBUG
            else
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
#endif
        }

        /// <summary>
        /// Call Show Message with a message.
        /// </summary>
        /// <param name="messageCode"></param>
        /// <returns></returns>
        public static async Task ShowErrorMessage(string message)
        {
            await ShowError(message);
        }

        public static async Task<bool> ShowDialog(string message, string title, string buttonOk, string buttonCancel = null)
        {
            message = CorrectForReturnChar(message);
            if (ShowDialogDelegate != null)
            {
                return await ShowDialogDelegate(message, title, buttonOk, buttonCancel);
            }
            else
            {
                await ShowMessageDelegate(message, title);
                return false;
            }
        }

        private static string CorrectForReturnChar(string message)
        {
            return message.Replace(@"\n", "\r\n");
        }
    }
}
