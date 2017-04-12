using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Linq;

namespace Common.Model
{
    /// <summary>
    /// Main entity to hold expense information.
    /// </summary>
    public partial class msdyn_expense : Microsoft.Xrm.Sdk.Samples.Entity, System.ComponentModel.INotifyPropertyChanged
    {
        protected bool? hasReceipts;

        /// <summary>
        /// The maximum length for the msdyn_description field.
        /// </summary>
        public const int DescriptionLength = 100;

        /// <summary>
        /// CRM does an automatic conversion of the date fields to local time. In order to count for that, we need the following:
        /// - When reading from the entity, we can read as is since we expect all clients to correct all date times for time zone conversions.
        /// - When writing to the entity, convert the time portion of the date time field to 12pm to minimize likelihood of date change after conversion. 
        /// </summary>
        public DateTime? msdyn_TransactionDate_utc
        {
            get
            {
                return msdyn_TransactionDate;
            }
            set
            {
                if (value == null)
                {
                    msdyn_TransactionDate = null;
                    return;
                }

                // Ensure that the date time is stored in UTC 12:00.
                DateTime utcDateTime = value.Value;
                msdyn_TransactionDate = new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, 12, 0, 0, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Check if expense has receipts
        /// </summary>
        /// <returns></returns>
        public bool HasReceipts
        {
            get
            {
                if (hasReceipts == null)
                {
                    AliasedValue aliasedValue = this.GetAttributeValue<AliasedValue>("receiptsExtended.objectid");
                    if (ExpenseReceiptId != Guid.Empty && aliasedValue != null)
                    {
                        hasReceipts = ExpenseReceiptId != Guid.Empty;
                        this.Attributes.Remove("receiptsExtended.objectid");
                    }
                }
                return ((hasReceipts ?? false) ||
                       (this.msdyn_expense_Receipts != null
                       && this.msdyn_expense_Receipts.Count<Annotation>() > 0));
            }
            set
            {
                hasReceipts = value;
                this.OnPropertyChanged("HasReceipts");
            }
        }

        protected Guid? expenseReceiptId;
        public Guid ExpenseReceiptId
        {
            get
            {
                if (expenseReceiptId == null)
                {
                    AliasedValue aliasedValue = this.GetAttributeValue<AliasedValue>("receipts.msdyn_expensereceiptid");
                    if (aliasedValue != null)
                    {
                        expenseReceiptId = aliasedValue.Value as Guid?;
                        this.Attributes.Remove("receipts.msdyn_expensereceiptid");
                    }
                }
                return expenseReceiptId ?? Guid.Empty;
            }
            set
            {
                expenseReceiptId = value;
            }
        }

        protected bool? hasComments;
        /// <summary>
        /// Check if expense has comments
        /// </summary>
        /// <returns></returns>
        public bool HasComments
        {
            get
            {
                if (hasComments == null)
                {
                    AliasedValue aliasedValue = this.GetAttributeValue<AliasedValue>("notes.objectid");
                    if (aliasedValue != null)
                    {
                        hasComments = true;
                        this.Attributes.Remove("notes.objectid");
                    }
                }
                return (hasComments ?? false);
            }
            set
            {
                hasComments = value;
                this.OnPropertyChanged("HasComments");
            }
        }

        /// <summary>
        /// If the expense is in draft or rejected status it can be saved
        /// </summary>
        /// <returns></returns>
        public bool CanEdit()
        {
            return this.msdyn_ExpenseStatus == null || 
                (this.msdyn_ExpenseStatus.Value == (int)msdyn_expense_msdyn_expensestatus.Draft
              || this.msdyn_ExpenseStatus.Value == (int)msdyn_expense_msdyn_expensestatus.Rejected);
        }

        /// <summary>
        /// Encapsulate the value of msdyn_amount
        /// </summary>
        public decimal TransactionAmount
        {
            get
            {
                if(this.msdyn_Amount != null)
                {
                    return this.msdyn_Amount.Value;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (this.msdyn_Amount == null)
                {
                    this.msdyn_Amount = new Money();
                }
                this.msdyn_Amount.Value = value;
                this.OnPropertyChanged("msdyn_Amount");
                this.OnPropertyChanged("FormattedTransactionAmount");
            }
        }

        /// <summary>
        /// Encapsulate the value of msdyn_salestaxamount
        /// </summary>
        public decimal SalesTaxAmount
        {
            get
            {
                if (this.msdyn_Salestaxamount != null)
                {
                    return this.msdyn_Salestaxamount.Value;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (this.msdyn_Salestaxamount == null)
                {
                    this.msdyn_Salestaxamount = new Money();
                }
                this.msdyn_Salestaxamount.Value = value;
                this.OnPropertyChanged("msdyn_Salestaxamount");
                this.OnPropertyChanged("FormattedTransactionAmount");
            }
        }

        /// <summary>
        /// Format the amount with 2 decimals based on transaction currency code, for example: 125.05 USD
        /// </summary>
        public string FormattedTransactionAmount
        {
            get
            {                
                decimal amountWithTaxes = this.TransactionAmount + this.SalesTaxAmount;
                if (this.TransactionCurrency.ISOCurrencyCode != null)
                {
                    return String.Format("{0:N2} {1}", amountWithTaxes, this.TransactionCurrency.ISOCurrencyCode);
                }
                else
                {
                    return String.Format("{0:N2}", amountWithTaxes);
                }
            }
        }

        protected TransactionCurrency extendedTransactionCurrency;
        public TransactionCurrency TransactionCurrency
        {
            get
            {
                if (extendedTransactionCurrency == null)
                {
                    extendedTransactionCurrency = new TransactionCurrency();
                    if (TransactionCurrencyId != null)
                    {
                        extendedTransactionCurrency.Id = TransactionCurrencyId.Id ?? Guid.Empty;
                        extendedTransactionCurrency.Preview = TransactionCurrencyId.Name;
                    }

                    // set ISO currencyCode
                    AliasedValue aliasedValue = this.GetAttributeValue<AliasedValue>("transactioncurrency.isocurrencycode");
                    if (aliasedValue != null)
                    {
                        extendedTransactionCurrency.ISOCurrencyCode = aliasedValue.Value as string;
                        this.Attributes.Remove("transactioncurrency.isocurrencycode");
                        this.FormattedValues.Remove("transactioncurrency.isocurrencycode");
                    }
                }
                return extendedTransactionCurrency;
            }
            set
            {
                extendedTransactionCurrency = value;
                TransactionCurrencyId = new EntityReference(TransactionCurrency.EntityLogicalName, extendedTransactionCurrency.Id);
                this.OnPropertyChanged("FormattedTransactionAmount");
                this.OnPropertyChanged("TransactionCurrency");
            }
        }

        protected msdyn_expensecategory extendedExpenseCategory;
        public msdyn_expensecategory ExpenseCategory
        {
            get
            {
                if (extendedExpenseCategory == null)
                {
                    extendedExpenseCategory = new msdyn_expensecategory();
                    if (this.msdyn_ExpenseCategory != null)
                    {
                        extendedExpenseCategory.Id = this.msdyn_ExpenseCategory.Id ?? Guid.Empty;
                        extendedExpenseCategory.Preview = this.msdyn_ExpenseCategory.Name;
                    }
                    // set expensetype
                    AliasedValue aliasedValue = this.GetAttributeValue<AliasedValue>(msdyn_expensecategory.EntityLogicalName + ".msdyn_expensetype");
                    if (aliasedValue != null)
                    {
                        extendedExpenseCategory.msdyn_ExpenseType = aliasedValue.Value as OptionSetValue;
                        //Remove copy of Aliased value
                        this.Attributes.Remove(msdyn_expensecategory.EntityLogicalName + ".msdyn_expensetype");
                        this.FormattedValues.Remove(msdyn_expensecategory.EntityLogicalName + ".msdyn_expensetype");
                    }

                    // set receipt configuration
                    aliasedValue = this.GetAttributeValue<AliasedValue>(msdyn_expensecategory.EntityLogicalName + ".msdyn_receiptrequired");
                    if (aliasedValue != null)
                    {
                        extendedExpenseCategory.msdyn_ReceiptRequired = aliasedValue.Value as OptionSetValue;
                        this.Attributes.Remove(msdyn_expensecategory.EntityLogicalName + ".msdyn_receiptrequired");
                        this.FormattedValues.Remove(msdyn_expensecategory.EntityLogicalName + ".msdyn_receiptrequired");
                    }

                }
                return extendedExpenseCategory;
            }
            set
            {
                extendedExpenseCategory = value;
                EntityReference selectedCategoryReference = new EntityReference(msdyn_expensecategory.EntityLogicalName, extendedExpenseCategory.Id);
                selectedCategoryReference.Name = extendedExpenseCategory.Preview;
                this.msdyn_ExpenseCategory = selectedCategoryReference;
                this.OnPropertyChanged("ExpenseCategory");
            }
        }

        /// <summary>
        /// 1:N msdyn_expense_Receipts. This is not a field in the entity
        /// </summary>
        public System.Collections.Generic.IEnumerable<Common.Model.Annotation> msdyn_expense_Receipts
        {
            get;
            set;
        }
    }
}
