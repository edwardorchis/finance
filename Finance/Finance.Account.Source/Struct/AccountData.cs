using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Source.Struct
{
    public class AccountData
    {
        public BeginBalance beginBalance { set; get; }

        public AccountBalance accountBalance { set; get; }
    }
}
