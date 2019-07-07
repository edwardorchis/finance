using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.Service;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class SerialNoController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(SerialNoController));
        SerialNoService service = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = SerialNoService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        [HttpPost]
        public FinanceResponse Get(SerialNoRequest request)
        {
            long id = service.Get((SerialNoKey)request.SerialKey, request.Ex);
            return CreateIdResponse(id);
        }
    }
}
