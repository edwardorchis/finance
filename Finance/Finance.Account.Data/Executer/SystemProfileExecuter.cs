using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class SystemProfileExecuter : DataExecuter, ISystemProfileExecuter
    {
        const int CATEGORY =(int) SystemProfileCategory.Account;

        public int GetInt(SystemProfileKey key)
        {
            var rsp = GetString(key);
            int result = 0;
            if (int.TryParse(rsp, out result))
            {
                return result;
            }
            throw new FinanceAccountDataException(FinanceAccountDataErrorCode.FORMAT_ERROR);
        }

        public string GetString(SystemProfileKey key)
        {
            var rsp = Execute(new SystemProfileRequest { Category = CATEGORY, Key =(int)key });
            return rsp.Content.value;
        }

        public void Update(SystemProfileKey key, object value)
        {
            Execute(new SystemProfileUpdateRequest { Category=CATEGORY,Key= (int)key, Value=value.ToString()});
        }

        public List<MenuTableMap> GetMenuTables()
        {
            var rsp = Execute(new MenuTablesRequest());
            return rsp.Content;
        }

        public List<MenuTableMap> GetAllMenuTables()
        {
            var rsp = Execute(new AllMenuTablesRequest());
            return rsp.Content;
        }

        public void SaveMenuTable(MenuTableMap menu)
        {
            Execute(new MenuTableSaveRequest { Content = menu});
        }

        public void DeleteMenuItem(MenuTableMap menu)
        {
            Execute(new MenuTableDeleteRequest { Content = menu });
        }

        public List<AccessRight> GetAccessRight(long id)
        {
            var rsp = Execute(new AccessRightRequest { id = id });
            return rsp.Content;
        }

        public void SaveAccessRight(List<AccessRight> lst)
        {
            Execute(new AccessRightSaveRequest { Content = lst });
        }

       
    }
}
