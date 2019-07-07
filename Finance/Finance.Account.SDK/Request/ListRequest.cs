using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class ListRequest
    {
        public IDictionary<string, object> Filter { set; get; }

        public SortedList<string,OrderByRule> OrderBy { set; get; }
    }
}
