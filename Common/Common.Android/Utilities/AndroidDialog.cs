using Android.App;
using Android.Content;
using Common.Utilities.Resources;
using System.Threading.Tasks;

namespace Common.Android.Utilities
{
    public class AndroidDialog
    {
        private Context context;
        public Context Context
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
            }
        }

        private static AndroidDialog current;
        public static AndroidDialog Current
        {
            get
            {
                if (current == null)
                {
                    current = new AndroidDialog();
                }
                return current;
            }
        }

        private TaskCompletionSource<bool> dialogTcs;

        private AndroidDialog() { }

        public async Task ShowMessageAction(string message, string title = null, string buttonOk = null)
        {
            buttonOk = buttonOk ?? AppResources.OK;

            dialogTcs = new TaskCompletionSource<bool>();

            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetMessage(message).SetTitle(title);
            builder.SetPositiveButton(buttonOk, new DialogOnClickListener(dialogTcs, true));

            AlertDialog dialog = builder.Create();
            dialog.Show();

            await dialogTcs.Task;

            dialog.Dismiss();
        }

        public async Task<bool> ShowDialogAction(string message, string title = null, string buttonOk = null, string buttonCancel = null)
        {
            buttonOk = buttonOk ?? AppResources.OK;
            buttonCancel = buttonCancel ?? AppResources.Cancel;

            dialogTcs = new TaskCompletionSource<bool>();

            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetMessage(message).SetTitle(title);
            builder.SetPositiveButton(buttonOk, new DialogOnClickListener(dialogTcs, true));
            builder.SetNegativeButton(buttonCancel, new DialogOnClickListener(dialogTcs, false));

            AlertDialog dialog = builder.Create();
            dialog.Show();

            return await dialogTcs.Task;
        }

        private class DialogOnClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            TaskCompletionSource<bool> tcs;
            bool dialogValue;

            public DialogOnClickListener(TaskCompletionSource<bool> tcs, bool dialogValue)
            {
                this.tcs = tcs;
                this.dialogValue = dialogValue;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                this.tcs.SetResult(dialogValue);
            }
        }
    }
}