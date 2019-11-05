using Finance.Account.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface IUserExecuter
    {
        string FindName(long id);

        List<User> List();

        void Login(long tid, string userName, string password);

        void HeartBeat();

        void Disable(long id);

        void Enable(long id);

        long Save(long id,string userName,string password);

        void ChangePassword(string oldpwd, string newpwd);
    }
}
