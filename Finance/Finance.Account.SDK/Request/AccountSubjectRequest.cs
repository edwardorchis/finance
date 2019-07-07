using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class AccountSubjectListRequest : IFinanceRequest<AccountSubjectListResponse>
    {
        public string Method
        {
            get { return "/accountsubject/list"; }
        }

        public int Status { set; get; }
    }

    public class AccountSubjectDeleteRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method => "/accountsubject/delete";
    }

    public class AccountSubjectSaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/accountsubject/save";

        public AccountSubject Content { set; get; }
    }

    public class AccountSubjectRequest : IdRequest, IFinanceRequest<AccountSubjectResponse>
    {
        public string Method => "/accountsubject/";
    }

    public class AccountSubjectSetStatusRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method => "/accountsubject/setstatus";
        
        public  int Status { set; get; }
    }
}
