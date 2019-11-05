using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.SDK.Utils;
using Finance.Account.Service;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Finance.Controller
{
    public class UserController: FinanceController
    {
        UserService service = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            service = UserService.GetInstance(controllerContext.Request.Properties);
            base.Initialize(controllerContext);
        }

        public FinanceResponse Login(UserRequest request)
        {
            long userId = 0;
            var userName = request.UserName;
            var password = CryptInfoHelper.GetDecrypte(request.PassWord);
            password = CryptInfoHelper.MD5Encode(password);

            userId = service.UserVerification(userName, password);
            if (userId == 0)
            {
                throw new FinanceException(FinanceResult.AUTHENTICATION_ERROR);
            }

            var tid = request.Tid;

            var token=OAuth2Handler.CreateToken(userId,userName,tid, DateTime.Now);
            return new UserResponse { UserId=userId,UserName = userName,Token=token };
        }

        public HeartBeatResponse Heartbeat(HeartBeatRequest request)
        {
            var lastTimeStamp =  request.LastTimeStamp;

            var lst = service.GetNeedRefreshTimeStampArticles(lastTimeStamp);
            List<long> taskList = new List<long>();
            if (lst.Contains((int)TimeStampArticleEnum.AccountSubject))
                taskList.Add((long)HeartBeatTask.RefreshAccountSubjectTask);
            if (lst.Contains((int)TimeStampArticleEnum.UserList))
                taskList.Add((long)HeartBeatTask.RefreshUserListTask);
            if (lst.Contains((int)TimeStampArticleEnum.Auxiliary))
                taskList.Add((long)HeartBeatTask.RefreshAuxiliaryTask);
            return new HeartBeatResponse { TimeStamp = CommonUtils.TotalMilliseconds(), TaskList = taskList};
        }

        public IdResponse Save(UserSaveRequest request)
        {
            var id = request.Id;
            var userName = request.UserName;
            var password = CryptInfoHelper.GetDecrypte(request.PassWord);
            password = CryptInfoHelper.MD5Encode(password);
            if (id == 0)
            {
                service.AddUser(userName, password);
            }
            else
            {
                service.ModifyUser(id,userName,password);
            }

            return new IdResponse { id = id };
        }

        public FinanceResponse Enable(UserEnableRequest request)
        {
            service.Enable(request.Id, request.IsDeleted);
            return CreateResponse(FinanceResult.SUCCESS);
        }

        public UserListResponse List(UserListRequest request)
        {
            var lst = service.List();

            return new UserListResponse { Content = lst};
        }

        public FinanceResponse ChangePassword(UserChangePasswordRequest request)
        {
            service.ChagePassword(request.Id, 
                CryptInfoHelper.MD5Encode(CryptInfoHelper.GetDecrypte(request.OldPwd)), 
                CryptInfoHelper.MD5Encode(CryptInfoHelper.GetDecrypte(request.NewPwd)));
            return CreateResponse(FinanceResult.SUCCESS);
        }

    }
}
