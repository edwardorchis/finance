using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.Data.Model;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Finance.Account.SDK.Response;
using Finance.Utils;
using Newtonsoft.Json;

namespace Finance.Account.Data.Executer
{
    public class VoucherExecuter : DataExecuter,IVoucherExecuter
    {
        public long Save(Voucher voucher)
        {
            var rsp = Execute(new VoucherSaveRequest { Content=voucher });
            return rsp.id;
        }

        public void Delete(long id)
        {
            Execute(new VoucherDeleteRequest { id=id });
        }

        public Voucher Find(long id, LINKED lniked = LINKED.CURRENT)
        {
            var rsp= Execute(new VoucherRequest { id = id ,Linked = (int)lniked});
            return rsp.Content;
        }

        public List<Voucher> List(IDictionary<string,object> filter)
        {
            var rsp = Execute(new VoucherListRequest { Filter=filter});
            return rsp.Content;
        }

        public void Check(long id)
        {
            Execute(new VoucherCheckRequest { id=id});
        }

        public void UnCheck(long id)
        {
            Execute(new VoucherUnCheckRequest { id = id });
        }

        public void Cancel(long id)
        {
            Execute(new VoucherCancelRequest { id = id });
        }

        public void UnCancel(long id)
        {
            Execute(new VoucherUnCancelRequest { id = id });
        }

        public void Post(long id)
        {
            Execute(new VoucherPostRequest { id = id });
        }

        public void UnPost(long id)
        {
            Execute(new VoucherUnPostRequest { id = id });
        }

        public void DoTest()
        {
            new Task(() =>
            {
                Execute(new VoucherDoTestRequest());
            }).Start();
        }

        public string Print(string fileName, long id)
        {
            return DownloadFile(fileName, new VoucherPrintRequest { id = id , FileName = fileName});
        }

    }
}
