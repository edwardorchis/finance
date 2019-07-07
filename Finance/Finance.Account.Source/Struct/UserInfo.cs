using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Source.Struct
{
    public class UserInfo
    {
        public long userId { set; get; }
        public string userName { set; get; }
        public string passWord { set; get; }
        public int isDeleted { set; get; }
    }
}
