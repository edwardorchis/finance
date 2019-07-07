using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class AccountBalanceResponse : FinanceResponse
    {
        public List<AccountAmountItem> ListBeginBalnace { set; get; }

        public List<AccountAmountItem> ListCurrentOccurs { set; get; }
    }
}
