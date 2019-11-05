using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UI
{
    public class FinanceDelegateEventHandler
    {
        public delegate void FinanceNotifyEventHandler(FinanceNotifyEventArgs e);

        public delegate void AfterSaveEventHandler();

        public delegate bool FinanceMsgEventHandler(int msgType, Dictionary<string, string> paras);
    }
}
