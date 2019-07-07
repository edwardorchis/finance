using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{
    public class CashflowSheetItem
    {
        public string Name { set; get; }
        public string LineNo { set; get; }
        public decimal Amount { set; get; }
        public int Flag { set; get; }
    }

    public class ProfitSheetItem: CashflowSheetItem
    {       
        public decimal Amount2 { set; get; }
    }
}
