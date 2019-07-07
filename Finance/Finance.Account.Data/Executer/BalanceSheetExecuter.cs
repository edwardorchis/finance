using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class BalanceSheetExecuter : DataExecuter, IBalanceSheetExecuter
    {
        public Dictionary<string, string> Calc(IDictionary<string, object> filter,Dictionary<string, string> template)
        {
            var rsp = Execute(new BalanceSheetCalcRequest { filter= filter, template = template });
            return rsp.Content;
        }

       
    }
}
