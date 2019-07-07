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
using Finance.Account.SDK.Request;
using Finance.Account.Service;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class AccountBalanceController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(AccountBalanceController));
        AccountBalanceService service = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = AccountBalanceService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }
      
        public FinanceResponse Query(AccountBalanceRequest request)
        {
            if (request == null)
                throw new FinanceException(FinanceResult.NULL);
            var beginYear = request.BeginYear;
            var beginPeriod = request.BeginPeriod;
            var endYear = request.EndYear;
            var endPeriod = request.EndPeriod;

            var prevPeriod= CommonUtils.CalcPrevPeriod(new PeridStrunct { Year = beginYear, Period = beginPeriod });           
            List<AccountAmountItem> lstBegin = service.QuerySettled(prevPeriod.Year, prevPeriod.Period);
            List<AccountAmountItem> lstCurrent = service.QueryOccurs(beginYear,beginPeriod,endYear,endPeriod);

            return new AccountBalanceResponse { ListBeginBalnace= lstBegin, ListCurrentOccurs= lstCurrent };           
        }

        public FinanceResponse Settle()
        {
            service.Settle();
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
