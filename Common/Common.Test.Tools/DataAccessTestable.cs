using Common.Utilities.DataAccess;
using Microsoft.Xrm.Sdk.Query.Samples;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Common.Test.Tools
{
    /// <summary>
    /// Class used to mock behavior of the DataAccess class.
    /// </summary>
    public class DataAccessTestable : DataAccess
    {
        private Action<OrganizationRequest> executeHandler;
        private Action<QueryBase, IEnumerable<TypeInfo>, bool> retrieveEntitiesHander;
        private IEnumerable<Entity> retrieveEntityResults;

        /// <summary>
        /// Add an Action to be executed when Execute is called.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        public void addExecuteHandler(Action<OrganizationRequest> action)
        {
            executeHandler += action;
        }

        public override Task<OrganizationResponse> Execute(OrganizationRequest request)
        {
            this.executeHandler(request);

            OrganizationResponse response = new OrganizationResponse();
            return Task.FromResult(response);
        }

        public override Task<Guid> GetLoggedUserId(bool implicitAction = false)
        {
            return Task.FromResult(Guid.Empty);
        }

        /// <summary>
        /// Add an action to be executed when RetrieveEntities is called.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        public void addRetrieveEntitiesHandler(Action<QueryBase, IEnumerable<TypeInfo>, bool> action)
        {
            retrieveEntitiesHander += action;
        }

        public override Task<List<TEntity>> RetrieveEntities<TEntity>(QueryBase query, IEnumerable<TypeInfo> relatedEntities = null, bool implicitAction = false)
        {
            if (this.retrieveEntitiesHander != null)
            {
                this.retrieveEntitiesHander(query, relatedEntities, implicitAction);
            }

            List<TEntity> results;
            if (this.retrieveEntityResults == null)
            {
                results = new List<TEntity>();
            }
            else
            {
                results = new List<TEntity>(
                    from en in this.retrieveEntityResults
                    select (TEntity)en);
            }
            
            return Task.FromResult(results);
        }

        /// <summary>
        /// Allows overriding the result of RetrieveEntities.
        /// </summary>
        /// <param name="result">IEnumerable of entities to return from RetrieveEntities.</param>
        public void setRetrieveEntitiesResult(IEnumerable<Entity> result)
        {
            this.retrieveEntityResults = result;
        }
    }
}
