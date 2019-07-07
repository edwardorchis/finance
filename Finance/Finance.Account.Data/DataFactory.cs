using Finance.Account.Data.Executer;
using System;
using System.Threading;

namespace Finance.Account.Data
{
    public class DataFactory
    {
        static DataFactory factory = null;
        public static DataFactory Instance
        {
            get
            {
                if (factory == null)
                    factory = new DataFactory();
                return factory;
            }
        }

        Timer timer = null;

        public DataFactory()
        {
            timer = new Timer(new TimerCallback(Heartbeat), null,
                1000 * 60, 1000 * 60);//1分钟心跳
        }

        public delegate void HeartbeatTimeOutEventHandler();
        public HeartbeatTimeOutEventHandler HeartbeatTimeOutEvent;

        public delegate void HeartbeatTimeOutRecoverEventHandler();
        public HeartbeatTimeOutRecoverEventHandler HeartbeatTimeOutRecoverEvent;
        int timeOutCount = 0;
        public void Heartbeat(object a)
        {
            try
            {
                GetUserExecuter().HeartBeat();
                //if (timeOutCount >= 3)
                HeartbeatTimeOutRecoverEvent?.Invoke();
                timeOutCount = 0;
            }
            catch
            {
                timeOutCount++;
                //if(timeOutCount >= 3)
                HeartbeatTimeOutEvent?.Invoke();
            }
        }

        IAuxiliaryExecuter m_Auxiliary = null;
        public IAuxiliaryExecuter GetAuxiliaryExecuter()
        {
            if (m_Auxiliary == null)
                m_Auxiliary = new Executer.AuxiliaryExecuter();
            return m_Auxiliary;
        }

        IAccountSubjectExecuter m_AccountSubject = null;
        public IAccountSubjectExecuter GetAccountSubjectExecuter()
        {
            if (m_AccountSubject == null)
                m_AccountSubject = new Executer.AccountSubjectExecuter();
            return m_AccountSubject;
        }

        CacheHashtable m_CacheHashtable = null;
        public CacheHashtable GetCacheHashtable()
        {
            if (m_CacheHashtable == null)
                m_CacheHashtable = new CacheHashtable();
            return m_CacheHashtable;
        }

        IVoucherExecuter m_voucher = null;
        public IVoucherExecuter GetVoucherExecuter()
        {
            if (m_voucher == null)
                m_voucher = new VoucherExecuter();
            return m_voucher;
        }

        IUserExecuter m_user = null;
        public IUserExecuter GetUserExecuter()
        {
            if (m_user == null)
                m_user = new UserExecuter();
            return m_user;
        }

        ISerialNoExecuter m_serialNo = null;
        public ISerialNoExecuter GetSerialNoExecuter()
        {
            if (m_serialNo == null)
                m_serialNo = new SerialNoExecuter();
            return m_serialNo;
        }

        IBeginBalanceExecuter m_BeginBalance = null;
        public IBeginBalanceExecuter GetBeginBalanceExecuter()
        {
            if (m_BeginBalance == null)
                m_BeginBalance = new BeginBalanceExecuter();
            return m_BeginBalance;
        }

        ISystemProfileExecuter m_SystemProfile = null;
        public ISystemProfileExecuter GetSystemProfileExecuter()
        {
            if (m_SystemProfile == null)
                m_SystemProfile = new SystemProfileExecuter();
            return m_SystemProfile;
        }

        IBalanceSheetExecuter m_balanceSheetExecuter = null;
        public IBalanceSheetExecuter GetBalanceSheetExecuter()
        {
            if (m_balanceSheetExecuter == null)
                m_balanceSheetExecuter = new BalanceSheetExecuter();
            return m_balanceSheetExecuter;
        }

        IProfitSheetExecuter m_profitSheetExecuter = null;
        public IProfitSheetExecuter GetProfitSheetExecuter()
        {
            if (m_profitSheetExecuter == null)
                m_profitSheetExecuter = new ProfitSheetExecuter();
            return m_profitSheetExecuter;
        }

        IAccountBalanceExecuter m_accountBalanceExecuter = null;
        public IAccountBalanceExecuter GetAccountBalanceExecuter()
        {
            if (m_accountBalanceExecuter == null)
                m_accountBalanceExecuter = new AccountBalanceExecuter();
            return m_accountBalanceExecuter;
        }

        ICashflowExecuter m_cashflowExecuter = null;
        public ICashflowExecuter GetCashflowExecuter()
        {
            if (m_cashflowExecuter == null)
                m_cashflowExecuter = new CashflowExecuter();
            return m_cashflowExecuter;
        }

        ITemplateExecuter m_templateExecuter = null;
        public ITemplateExecuter GetTemplateExecuter()
        {
            if (m_templateExecuter == null)
                m_templateExecuter = new TemplateExecuter();
            return m_templateExecuter;
        }

        IUdefReportExecuter m_udefReportExecuter = null;
        public IUdefReportExecuter GetUdefReportExecuter()
        {
            if (m_udefReportExecuter == null)
                m_udefReportExecuter = new UdefReportExecuter();
            return m_udefReportExecuter;
        }

        IInterfaceExecuter m_interfaceExecuter = null;
        public IInterfaceExecuter GetInterfaceExecuter()
        {
            if (m_interfaceExecuter == null)
                m_interfaceExecuter = new InterfaceExecuter();
            return m_interfaceExecuter;
        }

        IAccoutCtlExecuter m_accoutCtlExecuter = null;
        public IAccoutCtlExecuter GetAccountCtlExecuter()
        {
            if (m_accoutCtlExecuter == null)
                m_accoutCtlExecuter = new AccoutCtlExecuter();
            return m_accoutCtlExecuter;


        }
    }
}
