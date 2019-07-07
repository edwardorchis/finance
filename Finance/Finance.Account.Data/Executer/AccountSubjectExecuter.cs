using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Utils;
using Newtonsoft.Json;

namespace Finance.Account.Data.Executer
{
    public class AccountSubjectExecuter : DataExecuter, IAccountSubjectExecuter
    {
        public void Delete(long id)
        {
            Execute(new AccountSubjectDeleteRequest { id = id });
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AccountSubjectList);
        }

        public AccountSubject Find(long id)
        {
            var obj = List().FirstOrDefault(a=>a.id==id);
            if (obj == null)
            {
                DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AccountSubjectList);
                obj = List().FirstOrDefault(a => a.id == id);
            }
            if (obj == null)
                throw new FinanceAccountDataException(FinanceAccountDataErrorCode.DATA_NOT_EXIST);
            return obj;
        }

        public List<AccountSubject> List(int status = 0)
        {
            List<AccountSubject> result = null;
            if (!DataFactory.Instance.GetCacheHashtable().ContainsKey(CacheHashkey.AccountSubjectList))
            {
                var rsp = Execute(new AccountSubjectListRequest() { Status = status});
                result = rsp.Content;
                DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.AccountSubjectList, result);
            }
            else
            {
                result = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.AccountSubjectList);
            }
            return result;
        }

        public void Save(AccountSubject aso)
        {
            Execute(new AccountSubjectSaveRequest { Content = aso});
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AccountSubjectList);
        }

        public void SetStatus(long id, int status)
        {
            Execute(new AccountSubjectSetStatusRequest { id = id, Status = status });
        }
    }
}
