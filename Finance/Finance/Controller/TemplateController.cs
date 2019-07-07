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
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class TemplateController : FinanceController
    {
        TemplateSevice service = null;       
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = TemplateSevice.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        [HttpPost]
        public ExcelTemplateResponse GetExcelTemplate(ExcelTemplateRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new ExcelTemplateResponse { Content = service.FindTemplate(request.name) };
        }

        [HttpPost]
        public UdefTemplateResponse GetUdefTemplate(UdefTemplateRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new UdefTemplateResponse { Content = service.FindUdefTemplate(request.name) };
        }


        public FinanceResponse SaveUdeftemplate(UdefTemplateSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            service.SaveUdeftemplate(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }
        [HttpPost]
        public FinanceResponse DeleteUdeftemplate(UdefTemplateSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            service.DeleteUdeftemplate(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse ListCarriedForwardTempate(CarriedForwardTemplateRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new CarriedForwardTemplateResponse { Content = service.ListCarriedForwardTemplate(request.id) };
        }

        public FinanceResponse SaveCarriedForwardTemplate(CarriedForwardTemplateSaveRequest request)
        {
            if (request == null || request.Content == null || request.Content.Count  == 0)
                throw new FinanceException(FinanceResult.NULL);
            service.SaveCarriedForwardTemplate(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
