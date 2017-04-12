using System;
using System.Collections.Generic;

namespace Common.Model.Extension
{
    public class ReferenceData
    {
        public Dictionary<Guid, BaseEntity> DataById { get; protected set; }
        public Dictionary<string, BaseEntity> DataByName { get; protected set; }

        /// <summary>
        /// Create a dictionary by id and dictionary by name
        /// </summary>
        /// <param name="data"></param>
        public ReferenceData(ICollection<BaseEntity> data)
        {
            DataById = new Dictionary<Guid, BaseEntity>();
            DataByName = new Dictionary<string, BaseEntity>();

            if (data != null)
            {
                foreach (BaseEntity entity in data)
                {
                    this.AddData(entity);
                }
            }
        }

        /// <summary>
        /// Add new data in the reference data collection
        /// </summary>
        /// <param name="entity">The entity for which to add data.</param>        
        public void AddData(BaseEntity entity)
        {
            if (entity != null)
            {
                Guid keyById = entity.Id;
                if (!DataById.ContainsKey(keyById))
                {
                    DataById.Add(keyById, entity);
                }
                else
                {
                    DataById[keyById] = entity;
                }

                string preview = entity.Preview ?? String.Empty;
                if (!DataByName.ContainsKey(preview))
                {
                    entity.Index = DataByName.Count;
                    DataByName.Add(preview, entity);
                }
                else
                {
                    DataByName[preview] = entity;
                }
            }
        }

        /// <summary>
        /// If there is data, return first as default
        /// </summary>
        /// <returns></returns>
        public BaseEntity GetDefault()
        {
            foreach (BaseEntity data in DataById.Values)
            {
                return data;
            }
            return null;
        }
    }
}
