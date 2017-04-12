using Common.Model;
using Common.Utilities.DataAccess;
using Microsoft.Xrm.Sdk.Query.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSA.Expense.ViewModel
{
    public static class ExpenseHelper
    {
        /// <summary>
        /// Retrieves the bookable resource corresponding to the user provided. Null if no 
        /// corresponding bookable resource found.
        /// </summary>
        /// <param name="userId">The CRM GUID of the user.</param>
        /// <returns>The <c>Bookable Resource</c> record or null.</returns>
        public static async Task<BookableResource> GetBookableResourceForUser(Guid userId)
        {
            BookableResource bookableResource = null;
            DataAccess dataAccess = new DataAccess();

            if (userId != null && userId != Guid.Empty)
            {
                QueryExpression queryExpression = new QueryExpression(BookableResource.EntityLogicalName);
                queryExpression.ColumnSet = new ColumnSet("bookableresourceid", "userid");

                ConditionExpression crmUserExpression = new ConditionExpression(BookableResource.EntityLogicalName, "userid", ConditionOperator.Equal, userId);
                queryExpression.Criteria = new FilterExpression();
                queryExpression.Criteria.AddCondition(crmUserExpression);
                List<BookableResource> bookableResourceList = await dataAccess.RetrieveEntities<BookableResource>(queryExpression, null, false);

                if (bookableResourceList != null)
                {
                    bookableResource = bookableResourceList.FirstOrDefault<BookableResource>();
                }
            }

            return bookableResource;
        }
    }
}
