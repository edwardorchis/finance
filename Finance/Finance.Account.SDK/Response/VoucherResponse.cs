using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class VoucherResponse:FinanceResponse
    {
        public Voucher Content { set; get; }
    }

    public class VoucherListResponse : FinanceResponse
    {
        public List<Voucher> Content { set; get; }
    }

    public class VoucherPrintResponse : FinanceResponse
    {
        public string Content { set; get; }
    }
}
