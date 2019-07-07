using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class BeginBalanceListResponse : FinanceResponse
    {
        public List<BeginBalance> Content { set; get; }
    }
}
