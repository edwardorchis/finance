using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Finance
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        ILogger logger = Logger.GetLogger(typeof(WebApiExceptionFilterAttribute));
        //重写基类的异常处理方法
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //1.异常日志记录（正式项目里面一般是用log4net记录异常日志）
            logger.Error(actionExecutedContext.Exception);

            //2.返回调用方具体的异常信息
            if (actionExecutedContext.Exception is FinanceException)
            {
                FinanceResponse fex = new FinanceResponse();
                fex.Result = actionExecutedContext.Exception.HResult;
                fex.ErrMsg = actionExecutedContext.Exception.Message;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, fex);
            }          
            base.OnException(actionExecutedContext);
        }
    }
}
