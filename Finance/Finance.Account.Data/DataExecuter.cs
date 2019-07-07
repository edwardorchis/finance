using Finance.Account.SDK;
using Finance.Account.SDK.Utils;
using System;
using System.Configuration;

namespace Finance.Account.Data
{
    public class DataExecuter: AutoRetryClient
    {
        const string APPKEY = "FinanceClient";
        const string VER = "1.0";
        const string SECRET = "lhKL@ti&KukX5FmB0o5P0TABaXhF8I!T";

        protected override string Token
        {
            get
            {
                var result = "";
                try
                {
                    result = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.Token);
                }
                catch (FinanceAccountDataException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return result;
            }
        }

        protected override string Appkey => APPKEY;

        protected override string Ver => VER;

        protected override string Secret => SECRET;

        protected override string RootPath => ConfigurationManager.AppSettings["service_url"];

        static ILogger logger = new ClientLogger();

        public DataExecuter()
        {
            SetLoger(logger);
        }

    }

    public class ClientLogger : ILogger
    {
        public void Error(string message)
        {
            //Console.WriteLine(message);
        }

        public void Info(string message)
        {
            //Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            //Console.WriteLine(message);
        }
    }
}
