using Common.Utilities.DataAccess.ServiceAccess;
using Common.Utilities.Resources;
using Microsoft.Crm.Sdk.Messages.Samples;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Common.Utilities.DataAccess
{
    public enum DataAccessMode
    {
        Offline = 1,
        /// <summary>
        /// Actions performed under this option will be accessing cache, 
        /// if the data is in cache and not yet expired it will return the cached data, otherwise (first time access or expired data)
        /// it will try to hit the server and explicitly cached after get the response.
        /// </summary>
        Cache = 2,
        SyncConnection = 3,
        AsyncConnection = 4
    }

    public class DataAccess
    {
        public DataAccessMode AccessMode { get; set; }

        private WhoAmIResponse loggedUser;

        public DataAccess()
        {
            this.AccessMode = DataAccessMode.SyncConnection;
        }

        /// <summary>
        /// The first time the method will get called will connect with the backend and retrieve the current user, otherwise it will return the previous data.
        /// If is an implicit action, it means it was the app how started the request and so, if we are working offline we will ignore it 
        /// </summary>
        /// <returns></returns>
        public async Task<WhoAmIResponse> GetLoggedUser(bool implicitAction = false)
        {
            // If is the first time retrieve the current user
            if (loggedUser == null && !(implicitAction && this.AccessMode == DataAccessMode.Offline))
            {
                loggedUser = await this.Execute(new WhoAmIRequest()) as WhoAmIResponse;
            }
            return loggedUser;
        }

        /// <summary>
        /// Calls GetLoggedUser, if null it will return an empty Guid
        /// </summary>
        /// <param name="implicitAction"></param>
        /// <returns></returns>
        public virtual async Task<Guid> GetLoggedUserId(bool implicitAction = false)
        {
            WhoAmIResponse whoAmI = await this.GetLoggedUser(implicitAction);
            if (whoAmI != null)
            {
                return whoAmI.UserId;
            }
            return Guid.Empty;
        }

        protected OrganizationServiceFault CurrentServiceFault;
        /// <summary>
        /// Show correct message to the user based on error code. 
        /// Don't show the error to the user If the same error code is being displayed.
        /// </summary>
        /// <param name="serviceFault"></param>
        public async void OrganizationServiceFaultHandler(OrganizationServiceFault serviceFault, bool showErrorUser = true)
        {
            if (serviceFault != null &&
               (CurrentServiceFault == null ||
               (serviceFault.ErrorCode == 404 && CurrentServiceFault.ErrorCode != serviceFault.ErrorCode)))
            {
                // No network available
                this.AccessMode = DataAccessMode.Offline;
                if (showErrorUser)
                {
                    CurrentServiceFault = serviceFault;
                    if (String.IsNullOrEmpty(serviceFault.Message))
                    {
                        await MessageCenter.ShowErrorMessage(AppResources.NoNetwork);
                    }
                    else 
                    {
                        await MessageCenter.ShowError(serviceFault.Message);
                    }
                    CurrentServiceFault = null;
                }
            }
        }

        public async Task<TEntity[]> RestRetrieveEntities<TEntity>(string schemaName, ColumnSet columnSet = null, string filter = null)
            where TEntity : Entity
        {
            TEntity[] result = null;
            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                result = await OrganizationServiceProxy.Current.RestRetrieveCollection<TEntity>(schemaName, columnSet, filter);
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }
            return result;
        }

        /// <summary>
        /// Retrieve entities based on current query, if is an implicit action, it means it was the app how started the query and 
        /// if we are working offline we will ignore it 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="relatedEntities"></param>
        /// <param name="implicitAction"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> RetrieveEntities<TEntity>(QueryBase query, IEnumerable<TypeInfo> relatedEntities = null, bool implicitAction = false)
            where TEntity : Entity
        {
            List<TEntity> result = null;
            if (implicitAction && DataAccessMode.Offline == AccessMode)
            {
                // Is an app proactive action, so ignore it.
                return result;
            }

            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                result = await OrganizationServiceProxy.Current.RetrieveCollection<TEntity>(query, relatedEntities);
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }
                    
            return result;
        }

        /// <summary>
        /// Retrieves the specified columns of the Entity with the given Id
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to retrieve</typeparam>
        /// <param name="entityName">Logical name of the entity</param>
        /// <param name="id">If of the entity</param>
        /// <param name="columnSet">columns to retrieve</param>
        /// <returns></returns>
        public async Task<TEntity> RetrieveEntity<TEntity>(string entityName, Guid id, ColumnSet columnSet)
            where TEntity : Entity
        {
            TEntity result = default(TEntity);
            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                result = await OrganizationServiceProxy.Current.Retrieve(entityName, id, columnSet) as TEntity;
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }

            return result;
            
        }

        public async Task<Guid?> Create(Entity entity)
        {
            Guid? result = null;
            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                result = await OrganizationServiceProxy.Current.Create(entity);
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }
            return result;
        }

        public async Task<bool> Update(Entity entity)
        {
            bool result = false;
            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                await OrganizationServiceProxy.Current.Update(entity);
                result = true;
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }
            catch
            {
                result = false;
            }
            
            return result;
        }

        public async Task<bool> Delete(string entityName, Guid Id)
        {
            bool result = false;
            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                await OrganizationServiceProxy.Current.Delete(entityName, Id);
                result = true;
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }
            return result;
        }

        /// <summary>
        /// Asynchronously execute the specified OrganizationRequest.
        /// </summary>
        /// <param name="request">OrganizationRequest to execute.</param>
        /// <returns>OrganizationResponse</returns>
        public virtual async Task<OrganizationResponse> Execute(OrganizationRequest request)
        {
            OrganizationResponse result = null;
            try
            {
                // make sure there is a valid connection
                await Authentication.Authentication.Current.Authenticate();
                // Call server and wait for the response to update cache.
                result = await OrganizationServiceProxy.Current.Execute(request);
            }
            catch (OrganizationServiceFault serviceFault)
            {
                OrganizationServiceFaultHandler(serviceFault);
            }
                   
            return result;
        }

    }
}
