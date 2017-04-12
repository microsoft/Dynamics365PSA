using Microsoft.Xrm.Sdk.Query.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities.DataAccess.ServiceAccess
{
    public class FilterQuery
    {
        public FilterQuery(string name, QueryExpression queryExpression, ICollection<string> result)
        {
            this.Name = name;
            this.QueryExpression = queryExpression;
            this.Result = result;
        }
        public string Name { get; set; }

        public QueryExpression QueryExpression { get; set; }

        public ICollection<string> Result { get; set; }

        public bool HasData()
        {
            return (Result != null && Result.Count > 0);
        }
    }
}
