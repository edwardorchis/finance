using Finance.Utils;
using Finance.Account.Source;
using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

using System.Threading;
using System.ServiceProcess;
using System.Collections.Concurrent;

namespace Finance
{
    public class FinanceService : ServiceBase
    {
        public FinanceService()
        {
            InitializeComponent();
        }
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "FinanceService";
        }

        protected override void OnStart(string[] args)
        {
            if (255 == CommondHandler.Test())
            {
                CommondHandler.Process("init -f");
                CommondHandler.Process("act.init finance_demo -f");
            }
            StartService();
        }

        protected override void OnStop()
        {
            TreadSignQueue.Enqueue(1);
        }
      
        private static ConcurrentQueue<int> TreadSignQueue = new ConcurrentQueue<int>();
        static Thread webServiceThread = null;
        public static void StartService()
        {
            webServiceThread = new Thread(() => {
                try {
                    var baseAddress = ConfigHelper.Instance.XmlReadAppSetting("server_url");
                    Logger.GetLogger(typeof(FinanceService)).Error("Startup:" + baseAddress);
                    // Start OWIN host 
                    using (WebApp.Start<Startup>(url: baseAddress))
                    {
                        int sign = 0;                     
                        while (true)
                        {                           
                            if (TreadSignQueue.TryDequeue(out sign))
                            {
                                if (sign == 1) {
                                    Logger.GetLogger(typeof(FinanceService)).Error("ExitService");
                                    return;
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
                    Logger.GetLogger(typeof(FinanceService)).Error("StartService:" + ex.ToString());
                }
            });
            webServiceThread.Start();

        }
    }
    static class Program
    {
#if DBG
        ///// <summary>
        ///// 应用程序的主入口点。
        ///// </summary>
        [STAThread]
        static void Main()
        {
            FinanceService.StartService();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FrmServerManager());
        }
#else
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FinanceService()
            };
            ServiceBase.Run(ServicesToRun);
        }
#endif

    }
}
