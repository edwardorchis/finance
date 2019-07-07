using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.UI.Model
{
    public class VoucherListItem
    {
        public long id { set; get; }
        public string status { set; get; }
        public string date { set; get; }
        public string period { set; get; }
        public string wordno { set; get; }
        public string explanation { set; get; }
        public string subjectno { set; get; }
        public string subjectname { set; get; }
        public string amount { set; get; }
        public string debits { set; get; }
        public string credit { set; get; }
        public string cashier { set; get; }
        public string creater { set; get; }
        public string poster { set; get; }
        public string agent { set; get; }
        public string checker { set; get; }
        public string linkno { set; get; }
    }
}
