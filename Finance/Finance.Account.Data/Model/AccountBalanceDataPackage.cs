using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data.Model
{
    public class AccountBalanceDataPackage
    {
        public List<AccountAmountItem> ListBeginBalnace { set; get; }

        public List<AccountAmountItem> ListCurrentOccurs { set; get; }
    }
}
