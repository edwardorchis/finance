using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data.Model
{
    public class User
    {
        public long Id { set; get; }
        public string Name { set; get; }        
        public bool IsDeleted { set; get; }
    }
}
