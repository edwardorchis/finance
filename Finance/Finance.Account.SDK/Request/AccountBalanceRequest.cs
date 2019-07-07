using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class AccountBalanceRequest : IFinanceRequest<AccountBalanceResponse>
    {
        public string Method => "accountbalance/query";

        public int BeginYear { set; get; }
        public int BeginPeriod { set; get; }
        public int EndYear { set; get; }
        public int EndPeriod { set; get; }

    }


    public class AccountSettleRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "accountbalance/settle";
    }
}
