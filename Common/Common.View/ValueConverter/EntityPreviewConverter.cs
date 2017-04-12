using Common.ViewModel;
using Microsoft.Xrm.Sdk.Samples;
using System;
using Xamarin.Forms;

namespace Common.View.ValueConverter
{
    public class EntityPreviewConverter<TEntity> : IValueConverter
        where TEntity : Entity
    {
        /// <summary>
        /// Given an entityReference get the preview of the picker
        /// </summary>
        /// <param name="value">selected value</param>
        /// <param name="targetType">int to define the selected item</param>
        /// <param name="parameter">BaseEntityViewModel with ReferenceData</param>
        /// <param name="culture"></param>
        /// <returns>index in the picker</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EntityReference referenceEntity = value as EntityReference;
            BaseEntityViewModel viewModel = parameter as BaseEntityViewModel;
            
            if (referenceEntity != null && viewModel != null)
            {
                TEntity entity = viewModel.GetObjectById<TEntity>(referenceEntity.Id ?? Guid.Empty);
                if(entity != null){
                    return entity.Preview;
                }                
            }
            return String.Empty;
        }

        /// <summary>
        /// Given the preview get the reference entity
        /// </summary>
        /// <param name="value">index</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>entityReference</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ConvertBackFromPreview(value.ToString(), (BaseEntityViewModel)parameter);
            }

            return null;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="value"></param>
       /// <param name="viewModel"></param>
       /// <param name="?"></param>
       /// <returns></returns>
        public EntityReference ConvertBackFromPreview(string preview, BaseEntityViewModel viewModel)
        {
            EntityReference entityReference = null;
            if (preview != null && viewModel != null)
            {
                TEntity entity = viewModel.GetObjectByName<TEntity>(preview);
                if (entity != null)
                {
                    entityReference = new EntityReference(entity.LogicalName, entity.Id);
                    entityReference.Name = preview;
                }
            }
            return entityReference;
        }
    }
}
