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
    public class SystemProfileController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(SystemProfileController));
        SystemProfileService service = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = SystemProfileService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public FinanceResponse Find(SystemProfileRequest request)
        {
            var rsp = service.Find((SystemProfileCategory)request.Category, (SystemProfileKey)request.Key);
            return new SystemProfileResponse { Content=rsp };
        }

        public FinanceResponse List()
        {
            var rsp = service.List();
            return new SystemProfileListResponse { Content = rsp };
        }

        public FinanceResponse Udpate(SystemProfileUpdateRequest request)
        {
            service.Update((SystemProfileCategory)request.Category, (SystemProfileKey)request.Key, request.Value);
            return CreateResponse(FinanceResult.SUCCESS);
        }


        public FinanceResponse MenuTables(MenuTablesRequest request)
        {
            var rsp = service.MenuTables();
            return new MenuTablesResponse { Content = rsp };
        }

        public FinanceResponse AllMenuTables(AllMenuTablesRequest request)
        {
            var rsp = service.AllMenuTables();
            return new MenuTablesResponse { Content = rsp };
        }

        public FinanceResponse SaveMenu(MenuTableSaveRequest request)
        {
            service.SaveMenu(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public FinanceResponse DeleteMenu(MenuTableDeleteRequest request)
        {
            service.DeleteMenu(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public AccessRightResponse AccessRight(AccessRightRequest request)
        {
            var rsp = service.GetAccessRightList(request.id);
            return new AccessRightResponse { Content = rsp };
        }

        public FinanceResponse SaveAccessRight(AccessRightSaveRequest request)
        {
            service.SaveAccessRight(request.Content);
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
