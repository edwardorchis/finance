using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class AuxiliaryListResponse: FinanceResponse
    {
        public List<Auxiliary> Content { set; get; }
    }
}
