using Finance.Account.SDK;
using Finance.Account.SDK.Response;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public class UserService
    {
        static ILogger logger = Logger.GetLogger(typeof(UserService));
        private IDictionary<string, object> mContext;
        public UserService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static UserService GetInstance(IDictionary<string, object> ctx)
        {
            return new UserService(ctx);
        }

        public long UserVerification(string userName, string userPwd)
        {
            long userId = 0L;
            var objRnt = DBHelper.GetInstance(mContext).ExecuteScalar(string.Format("select _userId from _userInfo where _userName ='{0}' and _passWord = '{1}' and _isDeleted = 0",userName,userPwd));
            if (objRnt != null)
            {
                long.TryParse(objRnt.ToString(), out userId);
            }
            return userId;
        }


        public void UpdateTimeStampArticle(TimeStampArticleEnum article, dynamic tran = null)
        {
            if (tran == null)
            {
                var iRet = DBHelper.GetInstance(mContext).ExecuteSql(string.Format("update _TimeStampArticle set _value = {0} where _id = {1}",
                    CommonUtils.TotalMilliseconds(), (int)article));
                if (iRet == 0)
                    DBHelper.GetInstance(mContext).ExecuteSql(string.Format("insert into _TimeStampArticle (_value,_id )values({0},{1})",
                    CommonUtils.TotalMilliseconds(), (int)article));
            }
            else
            {
                var iRet = DBHelper.GetInstance(mContext).ExecuteSql(tran,string.Format("update _TimeStampArticle set _value = {0} where _id = {1}",
                  CommonUtils.TotalMilliseconds(), (int)article));
                if (iRet == 0)
                    DBHelper.GetInstance(mContext).ExecuteSql(tran, string.Format("insert into _TimeStampArticle (_value,_id )values({0},{1})",
                    CommonUtils.TotalMilliseconds(), (int)article));

            }
        }

        public List<int> GetNeedRefreshTimeStampArticles(long lastTimeStamp)
        {
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt("select _id from _TimeStampArticle where _value > " + lastTimeStamp);
            if (dt != null && dt.Rows.Count > 0)
            {
                var lst = dt.AsEnumerable().Select(dr=>Convert.ToInt32(dr["_id"])).ToList();
                return lst;
            }
            return new List<int>();
        }

        public long AddUser(string userName, string password)
        {
            long userId = 0;
            var tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                var bExist = DBHelper.GetInstance(mContext).Exist(tran, string.Format("select 1 from _userInfo where _userName ='{0}' ",userName));
                if (bExist)
                    throw new FinanceException(FinanceResult.RECORD_EXIST);
                SerialNoService serialNoService = new SerialNoService(mContext, tran);
                serialNoService.SerialKey = SerialNoKey.System;
                userId = serialNoService.First();
                DBHelper.GetInstance(mContext).ExecuteSql(
                    string.Format("insert into _userInfo (_userId,_userName,_passWord) values ({0},'{1}','{2}')", userId,userName,password));
                serialNoService.Update();
               UpdateTimeStampArticle(TimeStampArticleEnum.UserList, tran);

                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }
            return userId;
        }

        public void ModifyUser(long userId, string userName, string password)
        {
            var tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                var bExist = DBHelper.GetInstance(mContext).Exist(tran, string.Format("select 1 from _userInfo where _userId = {0} ", userId));
                if (!bExist)
                    throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);               
                DBHelper.GetInstance(mContext).ExecuteSql(
                    string.Format("update _userInfo set _userName = '{0}',_passWord='{1}' where _userId ={2}",  userName, password, userId));
                UpdateTimeStampArticle(TimeStampArticleEnum.UserList, tran);

                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }

        }

        public List<UserResponse> List()
        {
            var result = new List<UserResponse>();
            var dt = DBHelper.GetInstance(mContext).ExecuteDt("select _userId,_userName,_isDeleted from _userInfo");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var rsp = new UserResponse();
                    rsp.UserId = dr.Field<Int64>("_userId");
                    rsp.UserName = dr.Field<string>("_userName");
                    rsp.IsDeleted = dr.Field<int>("_isDeleted");
                    result.Add(rsp);
                }
            }
            return result;
        }


        public void Enable(long userId, int isDeleted)
        {
            var tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                var bExist = DBHelper.GetInstance(mContext).Exist(tran, string.Format("select 1 from  _userInfo where _userId = {0} ", userId));
                if (!bExist)
                    throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
                DBHelper.GetInstance(mContext).ExecuteSql(
                    string.Format("update _userInfo set _isDeleted ={0} where _userId ={1}", isDeleted,userId));
                UpdateTimeStampArticle(TimeStampArticleEnum.UserList, tran);

                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }
        }

        public void ChagePassword(long userId, string oldpwd, string newpwd)
        {
            var bExist = DBHelper.GetInstance(mContext).Exist(string.Format("select 1 from _userInfo where _userId = {0} and  _passWord = '{1}'", userId, oldpwd));
            if (!bExist)
                throw new FinanceException(FinanceResult.AUTHENTICATION_ERROR);
            DBHelper.GetInstance(mContext).ExecuteSql(
                string.Format("update _userInfo set _passWord='{1}' where _userId ={0}", userId, newpwd));
        }
    }
}
