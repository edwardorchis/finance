using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using System.Collections.Generic;

namespace Finance.Account.Data.Executer
{
    public class CashflowExecuter : DataExecuter, ICashflowExecuter
    {

        public List<CashflowSheetItem> ListSheet(Dictionary<string, string> filter)
        {
            var rsp = Execute(new CashflowSheetRequest { filter = filter});
            return rsp.Content;
        }


        public string DownloadFile(string fileName, Dictionary<string, string> filter)
        {
            return DownloadFile(fileName, new CashflowSheetExportRequest { filter = filter });
        }
    }
}
