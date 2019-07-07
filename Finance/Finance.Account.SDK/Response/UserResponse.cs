using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class UserResponse : FinanceResponse
    {
        public long UserId { set; get; }

        public string UserName { set; get; }

        public string Token { set; get; }

        public int IsDeleted { set; get; }
    }

    public class UserListResponse : FinanceResponse
    {

        public List<UserResponse> Content { set; get; }
    }

    public class HeartBeatResponse : FinanceResponse
    {
        public long TimeStamp { set; get; }

        public List<long> TaskList { set; get; }
    }
}
