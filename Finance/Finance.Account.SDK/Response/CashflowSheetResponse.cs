using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class CashflowSheetResponse : FinanceResponse
    {
        public List<CashflowSheetItem> Content { set; get; }
    }
}
