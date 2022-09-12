using System.Collections.Generic;
using System.Linq;
using Finance.Account.Data.Model;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Account.SDK.Utils;
using Finance.Utils;

namespace Finance.Account.Data.Executer
{
    public class UserExecuter : DataExecuter,IUserExecuter
    {
        public string FindName(long id)
        {
            if (id == 0)
                return "";
            return List().FirstOrDefault(u=>u.Id==id).Name;
        }

        public List<User> List()
        {
            List<User> users = null;
            try
            {
                users = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.UserList);
            }
            catch (FinanceAccountDataException fex)
            {
                if (fex.HResult == (int)FinanceAccountDataErrorCode.CACHE_DATA_NOT_EXIST)
                {
                    var rsp = Execute(new UserListRequest());
                    List<UserResponse> lst = rsp.Content;
                    users = new List<User>();
                    lst.ForEach(u =>
                    {
                        users.Add(new User { Id = u.UserId, Name = u.UserName ,IsDeleted =(u.IsDeleted == 1) });
                    });
                    DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.UserList, users);
                }
                else
                {
                    throw fex;
                }
            }
            return users;
        }

        public void Login(long tid, string userName, string password)
        {
            UserRequest request = new UserRequest();
            request.UserName = userName;
            request.PassWord = CryptInfoHelper.GetEncrypt(password);
            request.Tid = tid;
            var rsp = Execute(request);
            DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.UserId, rsp.UserId);
            DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.UserName, rsp.UserName);
            DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.Token, rsp.Token);
        }

        long lastTimeStamp = CommonUtils.TotalMilliseconds();

        public void HeartBeat()
        {
            var rsp = Execute(new HeartBeatRequest { LastTimeStamp = lastTimeStamp });
            lastTimeStamp = rsp.TimeStamp;
            List<long> taskList = rsp.TaskList;
            taskList.ForEach(task=> {                
                var taskType = (HeartBeatTask)task;
                switch (taskType)
                {
                    case HeartBeatTask.RefreshAccountSubjectTask:
                        DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AccountSubjectList);
                        break;
                    case HeartBeatTask.RefreshUserListTask:
                        DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.UserList);
                        break;
                    case HeartBeatTask.RefreshAuxiliaryTask:
                        DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AuxiliaryList);
                        break;
                }
            });
            
        }

        public void Disable(long id)
        {
            Execute(new UserEnableRequest { Id = id,IsDeleted = 1});
        }

        public void Enable(long id)
        {
            Execute(new UserEnableRequest { Id = id, IsDeleted = 0 });
        }

        public long Save(long id, string userName, string password)
        {
            var rsp = Execute(new UserSaveRequest { Id = id, UserName = userName,PassWord = CryptInfoHelper.GetEncrypt(password) });
            return rsp.id;
        }

        public void ChangePassword(string oldpwd, string newpwd)
        {
            Execute(new UserChangePasswordRequest {
                Id = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.UserId),
                OldPwd = CryptInfoHelper.GetEncrypt(oldpwd),
                NewPwd = CryptInfoHelper.GetEncrypt(newpwd)
            });
        }
    }
}
