using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class ProfitSheetExecuter : DataExecuter, IProfitSheetExecuter
    {
        public Dictionary<string, string> Calc(IDictionary<string, object> filter, Dictionary<string, string> template)
        {
            var rsp = Execute(new ProfitSheetCalcRequest { template = template,filter = filter });
            return rsp.Content;
        }

    }
}
