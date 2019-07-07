using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class SerialNoExecuter : DataExecuter, ISerialNoExecuter
    {
        public long Get(SerialNoKey key, string ex="")
        {
            var rsp = Execute(new SerialNoRequest{Ex = ex,SerialKey=(int)key});
            return rsp.id;
        }
    }
}
