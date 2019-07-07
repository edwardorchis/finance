using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class UdefReportController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(UdefReportController));
        UdefReportService service = null;       
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = UdefReportService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public FinanceResponse Query(UdefReportRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var result = service.Query(request.ProcName, request.Filter);
            if (result== null)
                throw new FinanceException(FinanceResult.SYSTEM_ERROR);
            return new UdefReportResponse { Content = result };
        }
    }
}
