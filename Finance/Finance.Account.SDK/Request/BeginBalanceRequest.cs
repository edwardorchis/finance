using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class BeginBalanceSaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/beginbalance/save"; }
        }

        public List<BeginBalance> Content { set; get; }
    }

    public class BeginBalanceListRequest : IFinanceRequest<BeginBalanceListResponse>
    {
        public string Method
        {
            get { return "/beginbalance/list"; }
        }
    }

    public class BeginBalanceFinishRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/beginbalance/finish"; }
        }
    }
}
