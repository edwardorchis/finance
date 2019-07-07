using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Source.Struct
{
    public class AccountCtlData
    {
        public AccountCtl accountCtl { set; get; }
        public AccountUser accountUser { set; get; }
    }


    public class AccountCtl
    {
        public long id { set; get; }
        public string no { set; get; }
        public string name { set; get; }
        public string connstr { set; get; }
        public DateTime createTime { set; get; }
    }

    public class AccountUser
    {
        public long id { set; get; }
        public string no { set; get; }
        public string name { set; get; }
        public string pwd { set; get; }
        public DateTime lastLoginTime { set; get; }
    }



}
