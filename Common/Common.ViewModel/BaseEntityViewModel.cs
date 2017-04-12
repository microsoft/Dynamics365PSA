using Common.Model.Extension;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.ViewModel
{
    public abstract class BaseEntityViewModel : BaseViewModel
    {
        protected Dictionary<Type, ReferenceData> ReferenceData;

        public bool HasPendingDataToSave { get; set; }

        public BaseEntityViewModel()
            : base()
        {
            this.ReferenceData = new Dictionary<Type, ReferenceData>();
            this.HasPendingDataToSave = false;
        }

        public virtual Task Initialize()
        {
            this.HasPendingDataToSave = false;
            return Task.FromResult(true);
        }

        /// <summary>
        /// Calls ForcedSave only if there is any change since last time the entity was saved
        /// </summary>
        /// <returns>True if there is not pending data to save or ForceSave returned true</returns>
        public virtual async Task<bool> Save()
        {
            if (this.HasPendingDataToSave)
            {
                return await this.ForcedSave();
            }
            else
            {
                return true;
            }
        }

        public abstract Task LoadReferenceData();

        /// <summary>
        /// Saves entity and updates HasPendingDataToSave value.
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> ForcedSave();

        /// <summary>
        /// Load reference based on current DataAccessMode and store it inside ReferenceData dictionary
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns"></param>
        /// <param name="referenceDataBehavior"></param>
        /// <param name="entityLogicalName"></param>
        /// <returns>True if query returns value</returns>
        protected async Task<bool> LoadReferenceData<TEntity>(QueryBase query)
            where TEntity : Entity
        {
            List<TEntity> result = await this.DataAccess.RetrieveEntities<TEntity>(query, null, true);
            if (result != null)
            {
                Type key = typeof(TEntity);
                List<Model.BaseEntity> referenceData = new List<Model.BaseEntity>();
                foreach (var entity in result)
                {
                    referenceData.Add((Model.BaseEntity)entity);
                }

                if (ReferenceData.ContainsKey(key))
                {
                    this.ReferenceData[key] = new ReferenceData(referenceData);
                }
                else
                {
                    ReferenceData.Add(key, new ReferenceData(referenceData));
                }
                return (result.Count > 0);
            }
            return false;
        }

        /// <summary>
        /// After any change in The entity mark HasPendingDataToSave to true;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Entity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!this.HasPendingDataToSave)
            {
                this.HasPendingDataToSave = true;
            }
        }

        /// <summary>
        /// Return the keys of the reference data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ICollection<string> GetReferenceKeys<TEntity>()
        {
            ReferenceData referenceData = this.GetReferenceData<TEntity>();
            if (referenceData != null)
            {
                return referenceData.DataByName.Keys;
            }
            return new List<string>();
        }

        /// <summary>
        /// Search in the ReferenceData dictionary the object for given type and key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity GetObjectByName<TEntity>(string key)
            where TEntity : Entity
        {
            ReferenceData referenceData = this.GetReferenceData<TEntity>();
            TEntity result = null;
            if (referenceData != null &&
                referenceData.DataByName.ContainsKey(key))
            {
                result = referenceData.DataByName[key] as TEntity;
            }
            return result;
        }

        public TEntity GetObjectById<TEntity>(Guid key)
            where TEntity : Entity
        {
            ReferenceData referenceData = this.GetReferenceData<TEntity>();
            TEntity result = null;
            if (referenceData != null &&
                referenceData.DataById.ContainsKey(key))
            {
                result = referenceData.DataById[key] as TEntity;
            }
            return result;
        }

        /// <summary>
        /// Returns reference data of selected entity type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected ReferenceData GetReferenceData<TEntity>()
        {
            Type key = typeof(TEntity);
            if (ReferenceData.ContainsKey(key))
            {
                return this.ReferenceData[key]; ;
            }
            return null;
        }

        protected ReferenceData GetOrCreateReferenceData<TEntity>()
        {
            ReferenceData result = this.GetReferenceData<TEntity>();
            if (result == null)
            {
                Type key = typeof(TEntity);
                List<Model.BaseEntity> referenceData = new List<Model.BaseEntity>();
                result = new ReferenceData(referenceData);
                ReferenceData.Add(key, result);
            }
            return result;
        }

        protected TEntity GetDefaultData<TEntity>()
            where TEntity : Entity
        {
            ReferenceData referenceData = this.GetReferenceData<TEntity>();
            if (referenceData != null)
            {
                return referenceData.GetDefault() as TEntity;
            }
            return null;
        }
    }
}
