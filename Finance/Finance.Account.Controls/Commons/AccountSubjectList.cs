using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Account.Controls.Commons.Consts;

namespace Finance.Account.Controls.Commons
{

    /// <summary>
    /// 缓存基础数据，由VoucherGrid对外暴露接口写入
    /// </summary>
    public class AccountSubjectList
    {
        static AccountSubjectList cache = null;
        public static AccountSubjectList Instance
        {
            get
            {
                if (cache == null)
                {
                    cache = new AccountSubjectList();
                }
                return cache;
            }
        }

        public static List<AccountSubjectObj> Get()
        {
            var list = Instance.AccountSubjectObjects;
            return list;
        }

        public static AccountSubjectObj Find(long accountSubjectId)
        {
            var accountSubjectObj = Instance.AccountSubjectObjects.FirstOrDefault(item => item.id == accountSubjectId);
            if (accountSubjectObj == null)
            {
                accountSubjectObj = new AccountSubjectObj();
            }
            return accountSubjectObj;
        }

        public static AccountSubjectObj FindByNo(string no)
        {
            var accountSubjectObj = Instance.AccountSubjectObjects.FirstOrDefault(item => item.no == no);
            if (accountSubjectObj == null)
            {
                accountSubjectObj = new AccountSubjectObj();
            }
            return accountSubjectObj;

        }

        List<AccountSubjectObj> AccountSubjectObjects
        {
            get
            {
                List<AccountSubjectObj> list = FinanceControlEventsManager.Instance.OnGetAccountSubjectListEvent();
                if (list == null)
                    list = new List<AccountSubjectObj>();
                return list;
            }
        }


    }
}
