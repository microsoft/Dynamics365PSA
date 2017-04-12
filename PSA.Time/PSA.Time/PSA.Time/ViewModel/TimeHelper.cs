using Common.Model;
using Common.Utilities.DataAccess;
using Microsoft.Xrm.Sdk.Query.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace PSA.Time.ViewModel
{
    public static class TimeHelper
    {
        public static async Tasks.Task<BookableResource> GetBookableResourceForUser(Guid userId)
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

        public static bool isBookableResourceValid(BookableResource bookableResource)
        {
            return bookableResource != null && bookableResource.BookableResourceId != null && bookableResource.BookableResourceId.GetValueOrDefault() != Guid.Empty;
        }
    }
}
