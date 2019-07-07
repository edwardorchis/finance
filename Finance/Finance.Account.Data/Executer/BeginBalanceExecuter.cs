using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class BeginBalanceExecuter : DataExecuter, IBeginBalanceExecuter
    {
        public void Finish()
        {
            Execute(new BeginBalanceFinishRequest());
        }

        public List<BeginBalance> List()
        {
            var rsp = Execute(new BeginBalanceListRequest());
            return rsp.Content;
        }

        public void Save(List<BeginBalance> content)
        {         
            Execute(new BeginBalanceSaveRequest { Content = content });
        }
    }
}
