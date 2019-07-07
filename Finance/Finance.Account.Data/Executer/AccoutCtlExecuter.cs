using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class AccoutCtlExecuter : DataExecuter, IAccoutCtlExecuter
    {
        public List<SampleItem> GetAccountList()
        {
            var rsp = Execute(new AccountCtlRequest());
            return rsp.Content;
        }
    }
}
