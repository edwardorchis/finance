using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.UI.Model
{
    public class BeginBalanceItem
    {
        public long Id { set; get; }
        public string No { set; get; }
        public string Name { set; get; }
        public bool IsReadOnly { set; get; }
        /// <summary>
        /// 借方
        /// </summary>
        public decimal DebitsAmount { set; get; }
        /// <summary>
        /// 贷方
        /// </summary>
        public decimal CreditAmount { set; get; }
    }
}
