using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class ProfitSheetController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(BalanceSheetController));
        ProfitSheetService service = null;       
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = ProfitSheetService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public FinanceResponse Calc(ProfitSheetCalcRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var dict = service.Calc(request.template);
            return new ProfitSheetCalcResponse { Content = dict };
        }

    }
}
