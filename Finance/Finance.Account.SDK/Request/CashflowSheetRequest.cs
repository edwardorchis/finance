using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class CashflowSheetRequest : IFinanceRequest<CashflowSheetResponse>
    {
        public string Method
        {
            get { return "/cashflow/listsheet"; }
        }

        public Dictionary<string, string> filter { set; get; }
    }

    public class CashflowSheetExportRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/cashflow/export"; }
        }

        public Dictionary<string, string> filter { set; get; }
    }
}
