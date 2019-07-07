using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface IAccountSubjectExecuter
    {
        List<AccountSubject> List(int status = 0);

        AccountSubject Find(long id);

        void Save(AccountSubject aso);

        void Delete(long id);

        void SetStatus(long id, int status);
    }
}
