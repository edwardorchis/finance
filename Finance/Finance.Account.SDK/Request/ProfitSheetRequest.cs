using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{ 
    public class ProfitSheetCalcRequest : IFinanceRequest<ProfitSheetCalcResponse>
    {
        public string Method
        {
            get { return "/profitsheet/calc"; }
        }
        public Dictionary<string,string> template { set; get; }
    }
}
