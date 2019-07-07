using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class CashflowController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(CashflowController));
        CashflowSevice service = null;
      
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = CashflowSevice.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public CashflowSheetResponse ListSheet(CashflowSheetRequest request)
        {
            Dictionary<string, string> filter = request.filter;
            var lst = service.ListSheet(filter);
            return new CashflowSheetResponse { Content = lst };
        }
        
        public HttpResponseMessage Export(CashflowSheetExportRequest request)
        {
            ExcelExportor exportor = new ExcelExportor(new CashflowExportHandler());
            Dictionary<string, string> filter = request.filter;           
            var lst = service.ListSheet(filter);
            var dt = EntityConvertor<CashflowSheetItem>.ToDataTable(lst);

            MemoryStream ms = new MemoryStream();
            exportor.Export(ms, dt, ".xls");

            string relativePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;     
            string sPath = Path.Combine(Path.GetFullPath(relativePath), "Cache");
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            string fileName = SerialNoService.GetUUID() + ".xls";
            string filePath = Path.Combine(sPath, fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();
            
            var stream = new FileStream(filePath, FileMode.Open);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            //System.IO.File.Delete(filePath);

            return response;
        }


        class CashflowExportHandler : IExportHandler
        {
            public void Encode(ref DataTable data)
            {
                data.Columns.Remove("_Flag");
                data.Columns["_Name"].Caption = "项目";
                data.Columns["_LineNo"].Caption = "行号";
                data.Columns["_Amount"].Caption = "金额";
            }
        }

    }
    
}
