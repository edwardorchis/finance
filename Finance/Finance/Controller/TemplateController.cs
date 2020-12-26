using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Account.Source.DTL;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class TemplateController : FinanceController
    {
        string Tid = "0";
        TemplateSevice service = null;       
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            Tid = controllerContext.Request.Properties["Tid"].ToString();
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
        [HttpPost]
        public FinanceResponse Upload(HttpRequestMessage request)
        {
            try
            {
                string relativePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string path = Path.GetFullPath(relativePath + ".Cache/") + SerialNoService.GetUUID() + ".xls";
                FileStream fs = new FileStream(path, FileMode.Append);
                BinaryWriter w = new BinaryWriter(fs);
                fs.Position = fs.Length;
                request.Content.CopyToAsync(fs).Wait();
                w.Close();
                fs.Close();

                var query = request.GetQueryNameValuePairs();
                string name = "";
                foreach (var kv in query)
                {
                    if (kv.Key == "name")
                    {
                        name = kv.Value;
                        break;
                    }
                }

                IImportHandler dtl = null;
                switch(name)
                {
                    case "BalanceSheet":
                        dtl = new BalanceSheetDTL();
                        break;
                    case "ProfitSheet":
                        dtl = new ProfitSheetDTL();
                        break;
                }
                if (dtl == null)
                {
                    return CreateResponse(FinanceResult.SYSTEM_ERROR);
                }

                dtl.SetFileName(path);
                ExcelImportor importor = new ExcelImportor(long.Parse(Tid), dtl);
                importor.Import();

                return CreateResponse(FinanceResult.SUCCESS);
            }
            catch
            {
                return CreateResponse(FinanceResult.SYSTEM_ERROR);
            }
        }
    }
}
