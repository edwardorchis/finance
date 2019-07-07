using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface ISerialNoExecuter
    {
        long Get(SerialNoKey key,string ex="");
    }
}
