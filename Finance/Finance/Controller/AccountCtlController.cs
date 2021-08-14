using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
using Finance.Account.Source;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class AccountCtlController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(AccountCtlController));
        AccountCtlService service = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = AccountCtlService.GetInstance();
            base.Initialize(controllerContext);
        }

        public FinanceResponse List()
        {
            System.Threading.Thread.Sleep(20);
            var dt = DBHelper.DefaultInstance.ExecuteDt("select _id, _no, _name from _AccountCtl order by _id");
            var lst = EntityConvertor<SampleItem>.ToList(dt);
            return new SampleItemListResponse { Content = lst};
        }
        public FinanceResponse Manage(AccountCtlManageRequest request)
        {
            Logger.RestLogger();
            string rsp = "";
            Logger.HookLogger((LogLevel level, string message) => {
                if (LogLevel.LevInfo != level)
                {
                    return;
                }
                rsp += message;               
            });     
            int ret = CommondHandler.Process(request.Params);
            Logger.RestLogger();
            return new FinanceResponse { Result = ret, ErrMsg = rsp}; 
        }
    }
}
