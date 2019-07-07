using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Source.Struct
{
    public class SerialNo
    {
        public int key { set; get; }

        public string ex { set; get; }

        public long number { set; get; }
    }


    public class TimeStampArticle
    {
        public long id { set; get; }
        public long value { set; get; }
    }
}
