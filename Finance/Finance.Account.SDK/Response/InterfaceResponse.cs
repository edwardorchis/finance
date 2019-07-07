using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class InterfaceResponse : FinanceResponse
    {
        public string Content { set; get; }
    }
    public class TaskResultResponse : FinanceResponse
    {
        public TaskResult Content { set; get; }
    }
}
