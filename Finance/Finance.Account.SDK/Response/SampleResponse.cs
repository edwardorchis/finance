using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class IdResponse: FinanceResponse
    {
        public long id { set; get; }
    }

    public class SampleItemResponse : FinanceResponse
    {
        public SampleItem Item { set; get; }
    }

    public class SampleItemListResponse : FinanceResponse
    {
        public List<SampleItem> Content { set; get; }
    }
}
