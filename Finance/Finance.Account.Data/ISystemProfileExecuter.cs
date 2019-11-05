using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface ISystemProfileExecuter
    {
        int GetInt(SystemProfileKey key);

        string GetString(SystemProfileKey key);

        void Update(SystemProfileKey key, object value);

        List<MenuTableMap> GetMenuTables();

        List<MenuTableMap> GetAllMenuTables();        

        void SaveMenuTable(MenuTableMap menu);

        void DeleteMenuItem(MenuTableMap menu);

        List<AccessRight> GetAccessRight(long id);

        void SaveAccessRight(List<AccessRight> lst);        
    }
}
