namespace Common.Model
{
    /// <summary>
    /// Currency in which a financial transaction is carried out.
    /// </summary>
    public partial class TransactionCurrency : Microsoft.Xrm.Sdk.Samples.Entity, System.ComponentModel.INotifyPropertyChanged
    {
        public override string Preview
        {
            get
            {
                return this.CurrencyName;
            }
            set
            {
                this.CurrencyName = value;
            }
        }
    }
}
