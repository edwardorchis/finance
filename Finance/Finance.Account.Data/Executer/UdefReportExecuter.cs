using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    class UdefReportExecuter : DataExecuter, IUdefReportExecuter
    {
        public UdefReportDataSet Query(string proc, Dictionary<string, object> filter)
        {
            var rsp = Execute(new UdefReportRequest { ProcName = proc, Filter = filter });
            return rsp.Content;
        }
    }
}
