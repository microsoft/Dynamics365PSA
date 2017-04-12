using Microsoft.Xrm.Sdk.Samples;
using System;

namespace Common.Model
{
    /// <summary>
    /// Primary entity to hold expense category information.
    /// </summary>
    public partial class msdyn_expensecategory : Entity, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Get the name of the transaction category
        /// </summary>
        public override string Preview
        {
            get
            {
                return this.msdyn_name ?? string.Empty;
            }
            set
            {
                if (this.msdyn_ExpenseCategoryuId == null)
                {
                    this.msdyn_ExpenseCategoryuId = new EntityReference();
                }
                this.msdyn_ExpenseCategoryuId.Name = value;
            }
        }

        /// <summary>
        /// Return true if the category has configuration set to Mandatory.
        /// </summary>
        /// <returns></returns>
        public bool IsReceiptMandatory()
        {
            return (msdyn_ReceiptRequired != null && (msdyn_expensecategorybehavior)Enum.ToObject(typeof(msdyn_expensecategorybehavior), msdyn_ReceiptRequired.Value)
                  == msdyn_expensecategorybehavior.Mandatory);
        }
    }
}
