using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
   

    public class BalanceSheetCalcRequest : IFinanceRequest<BalanceSheetCalcResponse>
    {
        public string Method
        {
            get { return "/balancesheet/calc"; }
        }
        public IDictionary<string, object> filter { set; get; }
        public IDictionary<string,string> template { set; get; }
    }
}
