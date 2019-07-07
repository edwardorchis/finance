using Finance.Account.SDK.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Request
{
    public class UserRequest : IFinanceRequest<UserResponse>
    {
        public string Method
        {
            get { return "/user/login"; }
        }

        public string UserName { set; get; }

        public string PassWord { set; get; }

        public long Tid { set; get; } 
    }

    public class HeartBeatRequest : IFinanceRequest<HeartBeatResponse>
    {
        public string Method => "/user/heartbeat";

        public long LastTimeStamp { set; get; }
    }

    public class UserSaveRequest : IFinanceRequest<IdResponse>
    {
        public string Method
        {
            get { return "/user/save"; }
        }

        public long Id { set; get; }

        public string UserName { set; get; }

        public string PassWord { set; get; }
    }

    public class UserListRequest : IFinanceRequest<UserListResponse>
    {
        public string Method
        {
            get { return "/user/list"; }
        }
    }

    public class UserEnableRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/user/enable";

        public long Id { set; get; }

        public int IsDeleted { set; get; }
    }
}
