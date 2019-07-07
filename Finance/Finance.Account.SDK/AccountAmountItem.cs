using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{
    public class BeginBalance: AccountAmountItem
    {
      
    }

    public class AccountAmountItem
    {
        /// <summary>
        /// 科目ID
        /// </summary>
        public long accountSubjectId { set; get; }
        /// <summary>
        /// 借方
        /// </summary>
        public decimal debitsAmount { set; get; }
        /// <summary>
        /// 贷方
        /// </summary>
        public decimal creditAmount { set; get; }
    }
}
