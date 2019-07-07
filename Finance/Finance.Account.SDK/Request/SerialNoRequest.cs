using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class SerialNoRequest : IFinanceRequest<IdResponse>
    {
        public string Method
        {
            get { return "/serialno/get"; }
        }

        public int SerialKey { set; get; }

        public string Ex{set;get;}
    }
}
