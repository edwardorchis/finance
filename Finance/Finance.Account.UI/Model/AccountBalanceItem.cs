using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.UI.Model
{
    public class AccountBalanceItem
    {
        public long Id { set; get; }
        public string No { set; get; }
        public string Name { set; get; }
        /// <summary>
        /// 期初余额
        /// </summary>
        public AccountAmountItem begin { set; get; }
        /// <summary>
        /// 本期发生
        /// </summary>
        public AccountAmountItem current { set; get; }
        /// <summary>
        /// 期末余额
        /// </summary>
        public AccountAmountItem end { set; get; }
    }
}
