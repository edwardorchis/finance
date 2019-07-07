using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class SystemProfileResponse : FinanceResponse
    {
        public SystemProfile Content { set; get; }
    }

    public class SystemProfileListResponse : FinanceResponse
    {
        public List<SystemProfile> Content { set; get; }
    }


    public class MenuTablesResponse : FinanceResponse
    {
        public List<MenuTableMap> Content { set; get; }

    }


    public class AccessRightResponse : FinanceResponse
    {
        public List<AccessRight> Content { set; get; }
    }
}
