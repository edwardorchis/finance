using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Utils
{
    public class FinanceApiException : Exception
    {
        public FinanceApiException()
        {
            
        }
        public FinanceApiException(string msg):base(msg)
        {

        }

        string msgEx = string.Empty;
        public FinanceApiException(int ErrCode, string msg)
        {
            base.HResult = ErrCode;
            msgEx = msg;
        }
        public override string Message => string.Format("ErrCode = {0} \r\n {1}", base.HResult,msgEx);

    }
}
