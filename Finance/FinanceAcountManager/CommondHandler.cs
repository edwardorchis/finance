using Finance.Account.SDK.Request;
using Finance.Account.SDK.Utils;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAcountManager
{
    class CommondHandler: InvokeProxy
    {        
        public static int Process(string commondString)
        {
            int iRet = 0;
            try
            {
                string[] tmp = commondString.Split(new char[] { ' ', '\t' }, options: StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 0)
                    return 0;
                string commondName = tmp[0];
                switch (commondName)
                {
                    case "exit":
                        return -1;
                    case "clear":
                        return 1;
                    default:
                        return Exec(commondString);
                }
            }
            catch (FinanceException fex)
            {
                iRet = 2;
            }
            catch (Exception ex)
            {
                iRet = 3;
            }
            return iRet;
        }

        static int Exec(string commondString)
        {
            var rsp = handler.Execute(new AccountCtlManageRequest { Params = commondString});
            Logger.GetLogger(typeof(CommondHandler)).Info(rsp.ErrMsg);
            return rsp.Result;
        }
   
        const string APPKEY = "FinanceClient";
        const string VER = "1.0";
        const string SECRET = "lhKL@ti&KukX5FmB0o5P0TABaXhF8I!T";

        protected override string Token
        {
            get
            {
                var result = "";               
                return result;
            }
        }

        protected override string Appkey => APPKEY;

        protected override string Ver => VER;

        protected override string Secret => SECRET;

        protected override string RootPath => ConfigHelper.Instance.XmlReadAppSetting("service_url"); 

        static Finance.Account.SDK.Utils.ILogger logger = new AcountManagerLogger();

        public CommondHandler()
        {
            SetLoger(logger);
        }
        static CommondHandler handler = new CommondHandler();
    }

    public class AcountManagerLogger : Finance.Account.SDK.Utils.ILogger
    {
        public void Error(string message)
        {
            Logger.GetLogger(typeof(CommondHandler)).Error(message);
        }

        public void Info(string message)
        {
            Logger.GetLogger(typeof(CommondHandler)).Info(message);
        }

        public void Warn(string message)
        {
            Logger.GetLogger(typeof(CommondHandler)).Warn(message);
        }
    }
}
