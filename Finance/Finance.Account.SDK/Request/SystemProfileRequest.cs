using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class SystemProfileRequest : IFinanceRequest<SystemProfileResponse>
    {
        public string Method => "/systemprofile/find";

        public int Category { set; get; }

        public int Key { set; get; }
    }

    public class SystemProfileListRequest : IFinanceRequest<SystemProfileListResponse>
    {
        public string Method => "/systemprofile/list";
    }


    public class SystemProfileUpdateRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/systemprofile/update";

        public int Category { set; get; }

        public int Key { set; get; }

        public string Value { set; get; }
    }

    public class MenuTablesRequest : IFinanceRequest<MenuTablesResponse>
    {
        public string Method => "/systemprofile/menutables";
    }

    public class AllMenuTablesRequest : IFinanceRequest<MenuTablesResponse>
    {
        public string Method => "/systemprofile/allmenutables";
    }

    public class MenuTableSaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/systemprofile/savemenu";

        public MenuTableMap Content { set; get; }
    }

    public class AccessRightRequest : IFinanceRequest<AccessRightResponse>
    {
        public string Method => "/systemprofile/accessright";

        public long id { set; get; }
    }

    public class AccessRightSaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/systemprofile/saveaccessright";

        public List<AccessRight> Content { set; get; }
    }
}
