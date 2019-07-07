using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{

    public class UdefReportRequest : IFinanceRequest<UdefReportResponse>
    {
        public string Method => "udefreport/query";
        public string ProcName { set; get; }
        public Dictionary<string, object> Filter { set; get; }
    }
}
