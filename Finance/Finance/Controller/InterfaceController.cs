using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class InterfaceController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(InterfaceController));
        InterfaceService service = null;
        
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = InterfaceService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public FinanceResponse ExecTask(InterfaceRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var result = service.ExecTask((ExecTaskType)request.TaskType, request.ProcName, request.Filter);            
            return new InterfaceResponse { Content = result };
        }

        [HttpPost]
        public FinanceResponse GetResult(InterfaceGetResultRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var result = service.GetResult(request.TaskId);
            return new TaskResultResponse { Content = result };
        }
    }
}
