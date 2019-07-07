using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.Data.Model;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;

namespace Finance.Account.Data.Executer
{
    public class AccountBalanceExecuter : DataExecuter, IAccountBalanceExecuter
    {
        public AccountBalanceDataPackage Query(int beginYear, int beginPeriod, int endYear, int endPeriod)
        {
            var request = new AccountBalanceRequest
            {
                BeginPeriod = beginPeriod,
                BeginYear=beginYear,
                EndPeriod=endPeriod,
                EndYear=endYear
            };
            AccountBalanceResponse rsp = Execute(request);
            AccountBalanceDataPackage package = new AccountBalanceDataPackage();
            package.ListBeginBalnace = rsp.ListBeginBalnace;
            package.ListCurrentOccurs = rsp.ListCurrentOccurs;
            return package;
        }

        public void Settle()
        {
            Execute(new AccountSettleRequest());
        }
    }
}
