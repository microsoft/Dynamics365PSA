namespace Common.Model
{
    /// <summary>
    /// Extension methods for project task preview string.
    /// </summary>
    public partial class msdyn_projecttask : Microsoft.Xrm.Sdk.Samples.Entity, System.ComponentModel.INotifyPropertyChanged
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
