//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Common.Model
{
    public enum msdyn_transactioncategoryhierarchyelementState
    {
        Active = 0,
        Inactive = 1,
    }

    /// <summary>
    /// Hierarchical relationship of the transaction category with a root node.
    /// </summary>
    public partial class msdyn_transactioncategoryhierarchyelement : Microsoft.Xrm.Sdk.Samples.Entity, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public msdyn_transactioncategoryhierarchyelement() : base(EntityLogicalName)
        {
        }

        public const string EntityLogicalName = "msdyn_transactioncategoryhierarchyelement";

        public const int EntityTypeCode = 10116;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Unique identifier of the user who created the record.
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference CreatedBy
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("createdby");
            }
        }

        /// <summary>
        /// Date and time when the record was created.
        /// </summary>
        public System.Nullable<System.DateTime> CreatedOn
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
            }
        }

        /// <summary>
        /// Unique identifier of the delegate user who created the record.
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference CreatedOnBehalfBy
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("createdonbehalfby");
            }
        }

        /// <summary>
        /// Sequence number of the import that created this record.
        /// </summary>
        public System.Nullable<int> ImportSequenceNumber
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
            }
            set
            {
                this.SetAttributeValue("importsequencenumber", value);
                this.OnPropertyChanged("ImportSequenceNumber");
            }
        }

        /// <summary>
        /// Unique identifier of the user who modified the record.
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference ModifiedBy
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("modifiedby");
            }
        }

        /// <summary>
        /// Date and time when the record was modified.
        /// </summary>
        public System.Nullable<System.DateTime> ModifiedOn
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
            }
        }

        /// <summary>
        /// Unique identifier of the delegate user who modified the record.
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference ModifiedOnBehalfBy
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("modifiedonbehalfby");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference msdyn_ChildCategory
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("msdyn_childcategory");
            }
            set
            {
                this.SetAttributeValue("msdyn_childcategory", value);
                this.OnPropertyChanged("msdyn_ChildCategory");
            }
        }

        /// <summary>
        /// The name of the custom entity.
        /// </summary>
        public string msdyn_name
        {
            get
            {
                return this.GetAttributeValue<string>("msdyn_name");
            }
            set
            {
                this.SetAttributeValue("msdyn_name", value);
                this.OnPropertyChanged("msdyn_name");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference msdyn_ParentCategory
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("msdyn_parentcategory");
            }
            set
            {
                this.SetAttributeValue("msdyn_parentcategory", value);
                this.OnPropertyChanged("msdyn_ParentCategory");
            }
        }

        /// <summary>
        /// Unique identifier for entity instances
        /// </summary>
        public System.Nullable<System.Guid> msdyn_transactioncategoryhierarchyelementId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>("msdyn_transactioncategoryhierarchyelementid");
            }
            set
            {
                this.SetAttributeValue("msdyn_transactioncategoryhierarchyelementid", value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
                this.OnPropertyChanged("msdyn_transactioncategoryhierarchyelementId");
            }
        }

        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.msdyn_transactioncategoryhierarchyelementId = value;
            }
        }

        /// <summary>
        /// Unique identifier for the organization
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.EntityReference OrganizationId
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.EntityReference>("organizationid");
            }
        }

        /// <summary>
        /// Date and time that the record was migrated.
        /// </summary>
        public System.Nullable<System.DateTime> OverriddenCreatedOn
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
            }
            set
            {
                this.SetAttributeValue("overriddencreatedon", value);
                this.OnPropertyChanged("OverriddenCreatedOn");
            }
        }

        /// <summary>
        /// Status of the Transaction Category Hierarchy Element
        /// </summary>
        public System.Nullable<Common.Model.msdyn_transactioncategoryhierarchyelementState> statecode
        {
            get
            {
                Microsoft.Xrm.Sdk.Samples.OptionSetValue optionSet = this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.OptionSetValue>("statecode");
                if ((optionSet != null))
                {
                    return ((Common.Model.msdyn_transactioncategoryhierarchyelementState)(System.Enum.ToObject(typeof(Common.Model.msdyn_transactioncategoryhierarchyelementState), optionSet.Value)));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((value == null))
                {
                    this.SetAttributeValue("statecode", null);
                }
                else
                {
                    this.SetAttributeValue("statecode", new Microsoft.Xrm.Sdk.Samples.OptionSetValue(((int)(value))));
                }
                this.OnPropertyChanged("statecode");
            }
        }

        /// <summary>
        /// Reason for the status of the Transaction Category Hierarchy Element
        /// </summary>
        public Microsoft.Xrm.Sdk.Samples.OptionSetValue statuscode
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.Samples.OptionSetValue>("statuscode");
            }
            set
            {
                this.SetAttributeValue("statuscode", value);
                this.OnPropertyChanged("statuscode");
            }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public System.Nullable<int> TimeZoneRuleVersionNumber
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<int>>("timezoneruleversionnumber");
            }
            set
            {
                this.SetAttributeValue("timezoneruleversionnumber", value);
                this.OnPropertyChanged("TimeZoneRuleVersionNumber");
            }
        }

        /// <summary>
        /// Time zone code that was in use when the record was created.
        /// </summary>
        public System.Nullable<int> UTCConversionTimeZoneCode
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<int>>("utcconversiontimezonecode");
            }
            set
            {
                this.SetAttributeValue("utcconversiontimezonecode", value);
                this.OnPropertyChanged("UTCConversionTimeZoneCode");
            }
        }

        /// <summary>
        /// Version Number
        /// </summary>
        public System.Nullable<long> VersionNumber
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
            }
        }

        /// <summary>
        /// N:1 lk_msdyn_transactioncategoryhierarchyelement_createdby
        /// </summary>
        public Common.Model.SystemUser lk_msdyn_transactioncategoryhierarchyelement_createdby
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.SystemUser>("lk_msdyn_transactioncategoryhierarchyelement_createdby", null);
            }
        }

        /// <summary>
        /// N:1 lk_msdyn_transactioncategoryhierarchyelement_createdonbehalfby
        /// </summary>
        public Common.Model.SystemUser lk_msdyn_transactioncategoryhierarchyelement_createdonbehalfby
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.SystemUser>("lk_msdyn_transactioncategoryhierarchyelement_createdonbehalfby", null);
            }
        }

        /// <summary>
        /// N:1 lk_msdyn_transactioncategoryhierarchyelement_modifiedby
        /// </summary>
        public Common.Model.SystemUser lk_msdyn_transactioncategoryhierarchyelement_modifiedby
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.SystemUser>("lk_msdyn_transactioncategoryhierarchyelement_modifiedby", null);
            }
        }

        /// <summary>
        /// N:1 lk_msdyn_transactioncategoryhierarchyelement_modifiedonbehalfby
        /// </summary>
        public Common.Model.SystemUser lk_msdyn_transactioncategoryhierarchyelement_modifiedonbehalfby
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.SystemUser>("lk_msdyn_transactioncategoryhierarchyelement_modifiedonbehalfby", null);
            }
        }

        /// <summary>
        /// N:1 msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ChildCategory
        /// </summary>
        public Common.Model.msdyn_transactioncategory msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ChildCategory
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.msdyn_transactioncategory>("msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ChildCa" +
                        "tegory", null);
            }
            set
            {
                this.SetRelatedEntity<Common.Model.msdyn_transactioncategory>("msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ChildCa" +
                        "tegory", null, value);
                this.OnPropertyChanged("msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ChildCa" +
                        "tegory");
            }
        }

        /// <summary>
        /// N:1 msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ParentCategory
        /// </summary>
        public Common.Model.msdyn_transactioncategory msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ParentCategory
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.msdyn_transactioncategory>("msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ParentC" +
                        "ategory", null);
            }
            set
            {
                this.SetRelatedEntity<Common.Model.msdyn_transactioncategory>("msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ParentC" +
                        "ategory", null, value);
                this.OnPropertyChanged("msdyn_msdyn_transactioncategory_msdyn_transactioncategoryhierarchyelement_ParentC" +
                        "ategory");
            }
        }

        /// <summary>
        /// N:1 organization_msdyn_transactioncategoryhierarchyelement
        /// </summary>
        public Common.Model.Organization organization_msdyn_transactioncategoryhierarchyelement
        {
            get
            {
                return this.GetRelatedEntity<Common.Model.Organization>("organization_msdyn_transactioncategoryhierarchyelement", null);
            }
        }
    }
}
