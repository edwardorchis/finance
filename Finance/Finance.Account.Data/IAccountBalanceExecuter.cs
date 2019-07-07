using Finance.Account.Data.Model;

namespace Finance.Account.Data
{
    public interface IAccountBalanceExecuter
    {
        AccountBalanceDataPackage Query(int beginYear, int beginPeriod, int endYear, int endPeriod);

        void Settle();
    }
}
