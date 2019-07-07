using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class AccountSubjectListResponse : FinanceResponse
    {
        public List<AccountSubject> Content { set; get; }
    }

    public class AccountSubjectResponse : FinanceResponse
    {
        public AccountSubject Content { set; get; }
    }
}
