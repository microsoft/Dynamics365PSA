using Common.Utilities.DataAccess;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Common.ViewModel
{
    /// <summary>
    /// Base class for ViewModel. It will provide a IsBusy property to indicate async process is happening and the DataAccess layer
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private DataAccess dataAccess;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public BaseViewModel()
        {
        }

        private string title;
        /// <summary>
        /// Title for a bound page.
        /// </summary>
        public virtual string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        private bool isBusy;
        /// <summary>
        /// IsBusy indicates if the view model is completing a long running operation.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Get or set the DataAccess used by this view model.
        /// </summary>
        public DataAccess DataAccess
        {
            get
            {
                if (this.dataAccess == null)
                {
                    // Create the default if it hasn't been.
                    this.dataAccess = new DataAccess();
                }
                return this.dataAccess;
            }
            set
            {
                this.dataAccess = value;
            }
        }

        // Used to notify UI that a property has changed.
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
