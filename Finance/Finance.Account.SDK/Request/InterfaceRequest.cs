using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{

    public class InterfaceRequest : IFinanceRequest<InterfaceResponse>
    {
        public string Method => "interface/exectask";
        public string ProcName { set; get; }
        public int TaskType { set; get; }
        public Dictionary<string, object> Filter { set; get; }
    }

    public class InterfaceGetResultRequest : IFinanceRequest<TaskResultResponse>
    {
        public string Method => "interface/getresult";
        public string TaskId { set; get; }
    }
}
