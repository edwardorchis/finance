using Finance.Utils;
using Finance.Account.SDK;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Account.SDK.Request;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class AccountSubjectController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(AccountSubjectController));
        AccountSubjectService service = null;    
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = AccountSubjectService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public FinanceResponse List(AccountSubjectListRequest request)
        {
            List<AccountSubject> lst = service.List(request.Status);
            return new AccountSubjectListResponse { Content = lst };           
        }

       
        public AccountSubjectResponse Find(long id)
        {
            var aso = service.Find(id);
            return new AccountSubjectResponse { Content = aso };
        }
        
        public FinanceResponse Save(AccountSubjectSaveRequest request)
        {
            service.Save(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }
       
        [HttpPost]
        public FinanceResponse Delete(long id)
        {
            service.Delete(id);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse SetStatus(AccountSubjectSetStatusRequest request)
        {
            service.SetStatus(request.id,request.Status);
            return CreateResponse(FinanceResult.SUCCESS);
        }

    
    }
}
