using Finance.Utils;
using Finance.Account.SDK;
using System.Web.Http;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using System.Threading;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.IO;
using System.Web;
using Finance.Account.Source;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace Finance.Controller
{
    public class VoucherController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(VoucherController));
        VoucherService service = null;
        LogService logService = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = new VoucherService(controllerContext.Request.Properties);
            logService = new LogService(controllerContext.Request.Properties, typeof(VoucherController));
            base.Initialize(controllerContext);
        }

        public FinanceResponse List(VoucherListRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            return new VoucherListResponse { Content =service.List(request.Filter) };           
        }

        public FinanceResponse Find(VoucherRequest request)
        {
            var voucher = new Voucher();
            var id =service.Linked(request.id,request.Linked);
            if (id == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            voucher.header = service.FindHeader(id);           
            voucher.entries = service.FindEntrys(id);
            voucher.udefenties = service.FindUdefEntrys(id);
            return new VoucherResponse { Content = voucher };
        }
        
        public FinanceResponse Save(VoucherSaveRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var voucher = request.Content;
            var id = voucher.header.id;
            if (id == 0)
            {
                id = service.Add(voucher);
                logService.Write(Operation.Add, voucher.header.word + "-" + voucher.header.no);
            }
            else
            {
                service.Update(voucher);
                logService.Write(Operation.Update, voucher.header.word + "-" + voucher.header.no);
            }
            return CreateIdResponse(id);
        }
       
        [HttpPost]
        public FinanceResponse Delete(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Delete(id);
            logService.Write(Operation.Delete, header.word + "-" + header.no);
            return CreateIdResponse(id);
        }

        public FinanceResponse Check(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Check(id);
            logService.Write(Operation.Check, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse UnCheck(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.UnCheck(id);
            logService.Write(Operation.UnCheck, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse Cancel(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Cancel(id);
            logService.Write(Operation.Cancel, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse UnCancel(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.UnCancel(id);
            logService.Write(Operation.UnCancel, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse Post(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.Post(id);
            logService.Write(Operation.Post, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse UnPost(long id)
        {
            var header = service.FindHeader(id);
            if (header == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            service.UnPost(id);
            logService.Write(Operation.UnPost, header.word + "-" + header.no);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse DoTest()
        {
            var uid = SerialNoService.GetUUID();
            int i = 1;
            while (i < 1000)
            {
                logger.Debug(uid + ":" + i);
                Thread.Sleep(10);
                i++;
            }
            return CreateResponse(FinanceResult.SUCCESS);
        }


        public HttpResponseMessage Print(VoucherPrintRequest request)
        {
            PrintTemplateInfo tmpInfo = new PrintTemplateInfo();
            tmpInfo.name = "凭证打印模板_v1.xlsx";
            tmpInfo.procName = "sp_voucher_print_v1";
            tmpInfo.id = request.id;
            PrintAssemble printAssemble = new PrintAssemble(tmpInfo, service);            
            string filePath = printAssemble.Package();

            var stream = new FileStream(filePath, FileMode.Open);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = request.FileName
            };

            //System.IO.File.Delete(filePath);

            return response;
        }

    }
}
