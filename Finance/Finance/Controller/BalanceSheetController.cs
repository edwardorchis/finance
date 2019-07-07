using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using System.Web.Http;

namespace Finance.Controller
{
    public class BalanceSheetController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(BalanceSheetController));


        public FinanceResponse Calc(BalanceSheetCalcRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var dict = BalanceSheetService.GetInstance(Request.Properties).Calc(request.filter, request.template);
            return new BalanceSheetCalcResponse { Content = dict };
        }

    }
}
