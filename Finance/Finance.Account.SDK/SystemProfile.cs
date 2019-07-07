using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{
    /// <summary>
    /// 系统参数
    /// </summary>
    public class SystemProfile
    {
        public SystemProfileCategory category { set; get; }

        public SystemProfileKey key { set; get; }

        public string value { set; get; }

        public string description { set; get; }
    }


    public class MenuTableMap
    {       
        public string group{ set; get; }
        public string name { set; get; }
        public string header { set; get; }
        public string financeForm { set; get; }
        public int index { set; get; }
    }

    public class AccessRight
    {
        public long id { set; get; }
        public string group { set; get; }
        public string name { set; get; }
        public long mask { set; get; }
    }
}
