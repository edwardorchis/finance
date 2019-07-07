using Finance.Account.SDK;

namespace Finance.Account.Source.Struct
{
    /// <summary>
    /// 科目余额表
    /// 1、初始化完成时，把期初余额写入该表
    /// 2、结账后，写入该表    /// 
    /// </summary>
    public class AccountBalance: AccountAmountItem
    {
        public long year { set; get; }
        public long period { set; get; }
    }
}
