using Finance.Utils;
namespace Finance.Account.Service
{
    public class AccountCtlService
    {
        static ILogger logger = Logger.GetLogger(typeof(AccountCtlService));
        static AccountCtlService _self = null;       

        public static AccountCtlService GetInstance()
        {
            if (_self == null)
                _self = new AccountCtlService();
            return _self;
        }

    }
}
