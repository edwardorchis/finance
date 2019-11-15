using Finance.Account.Data.Model;
using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface IVoucherExecuter
    {
        long Save(Voucher voucher);       
        void Delete(long id);
        Voucher Find(long id,LINKED liked = LINKED.CURRENT);
        List<Voucher> List(IDictionary<string,object> filter);

        void Check(long id);
        void UnCheck(long id);
        void Cancel(long id);
        void UnCancel(long id);
        void Post(long id);
        void UnPost(long id);
        string Print(string fileName, long id);

        void DoTest();
    }
}
