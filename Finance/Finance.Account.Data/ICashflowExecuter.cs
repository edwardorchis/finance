using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface ICashflowExecuter
    {
        List<CashflowSheetItem> ListSheet(Dictionary<string, string> filter);
        string DownloadFile(string fileName,Dictionary<string, string> filter);
    }
}
