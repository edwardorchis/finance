using Finance.Utils;
using Finance.Account.Source;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;

namespace Finance
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread thread2 = new Thread(() => {
                var baseAddress = ConfigurationManager.AppSettings["server_url"];
                Console.WriteLine("Startup:" + baseAddress);
                // Start OWIN host 
                using (WebApp.Start<Startup>(url: baseAddress))
                {
                    Console.WriteLine("Finance is running...");
                    //ConsoleHelper.hideConsole();
                    while (true)
                    {
                        Console.Write("$ ");
                        string commond = Console.ReadLine();
                        if (!string.IsNullOrEmpty(commond))
                        {
                            switch (commond)
                            {
                                case "init":
                                    AccountCtlMain.Init();
                                    break;
                                case "clear":
                                    Console.Clear();
                                    break;
                                case "exit":
                                    return;
                            }
                        }
                    }
                }
            });
            thread2.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmServerManager());
        }
    }
}
