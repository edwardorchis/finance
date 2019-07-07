using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class BeginBalanceController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(BeginBalanceController));
        BeginBalanceSevice service = null;
        LogService logService = null;
       
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = BeginBalanceSevice.GetInstance(controllerContext.Request.Properties);
            logService = LogService.GetInstance(controllerContext.Request.Properties, typeof(BeginBalanceController));
            base.Initialize(controllerContext);
        }

        public FinanceResponse Save(BeginBalanceSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            service.Save(request.Content);
            logService.Write(Operation.Update,"");
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse List()
        {
            var lst= service.List();            
            return new BeginBalanceListResponse {Content=lst };
        }

        public FinanceResponse Finish()
        {
            service.Finish();
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
