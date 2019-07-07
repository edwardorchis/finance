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
        public Dictionary<string, string> Calc(Dictionary<string, string> template)
        {
            var rsp = Execute(new ProfitSheetCalcRequest { template = template });
            return rsp.Content;
        }

    }
}
