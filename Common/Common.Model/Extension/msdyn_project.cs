namespace Common.Model
{
    /// <summary>
    /// Extension methods for project preview string.
    /// </summary>
    public partial class msdyn_project : Microsoft.Xrm.Sdk.Samples.Entity, System.ComponentModel.INotifyPropertyChanged
    {
        public override string Preview
        {
            get
            {
                return this.msdyn_subject;
            }
            set
            {
                this.msdyn_subject = value;
            }
        }
    }
}
