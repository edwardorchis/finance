using Finance.Account.SDK;
using Finance.Account.SDK.Response;
using Finance.Properties;
using Finance.Utils;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using DotNetOpenAuth.Messaging;

namespace Finance
{
    public class FinanceController : ApiController
    {
        public FinanceResponse CreateResponse(FinanceResult result)
        {
            return new FinanceResponse { Result = (int)result };
        }

        public IdResponse CreateIdResponse(long id)
        {
            return new IdResponse { Result = (int)FinanceResult.SUCCESS, id = id };
        }

        public FinanceResponse CreateResponse(FinanceException ex)
        {
            FinanceResponse fex = new FinanceResponse();
            fex.Result = ex.HResult;
            fex.ErrMsg = ex.Message;
            return fex;
        }

        public virtual void InvokeInit(IDictionary<string, object> cookie)
        {
            HttpControllerContext context = new HttpControllerContext();
            context.Request = new HttpRequestMessage();
            context.Request.Properties.AddRange(cookie);
            Initialize(context);
        }

    }
}
