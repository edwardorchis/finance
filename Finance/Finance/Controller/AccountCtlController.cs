using Finance.Account.SDK;
using Finance.Account.SDK.Response;
using Finance.Account.Service;
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
    }
}
