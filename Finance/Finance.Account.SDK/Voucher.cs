using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finance.Account.SDK
{
    public class Voucher
    {
        public VoucherHeader header { set; get; }

        public List<VoucherEntry> entries { set; get; }

        public Dictionary<string, Dictionary<string, object>> udefenties { set; get; }
    }
}
