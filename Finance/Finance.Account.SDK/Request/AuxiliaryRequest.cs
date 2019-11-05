using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class AuxiliaryListRequest : IFinanceRequest<AuxiliaryListResponse>
    {
        public string Method
        {
            get { return "/auxiliary/list"; }
        }

        public int Type { set; get; }
    }

    public class AuxiliaryDeleteRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method => "/auxiliary/delete";
    }

    public class AuxiliarySaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/auxiliary/save";

        public Auxiliary Content { set; get; }
    }
}
