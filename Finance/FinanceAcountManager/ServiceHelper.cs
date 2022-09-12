using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceAcountManager
{
    class ServiceHelper
    {
        private static readonly ServiceHelper inst = new ServiceHelper();

        static ServiceHelper() { }
        private ServiceHelper() { }
        public static ServiceHelper Instance { get { return inst; } }

        private TaskFactory tf = new TaskFactory();
        ServiceController sc = null;
        const string SERVICENAME = "FinanceService";
        const string PROCESSNAME = "Finance";

        bool bStarting = false;
        public void StartService(Action<LogLevel, string> info)
        {
            if (bStarting)
            {
                info(LogLevel.LevInfo, "正在启动服务");
                return;
            }
            tf.StartNew(() =>
            {
                bStarting = true;
                if (sc == null)
                    sc = new ServiceController(SERVICENAME);
                sc.Refresh();
                if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    try
                    {
                        WaitForProcessClosed(PROCESSNAME, new TimeSpan(0, 0, 0, 30));
                        sc.Start();
                    }
                    catch { }
                }
                else if (sc.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    try
                    {
                        WaitForProcessClosed(PROCESSNAME, new TimeSpan(0, 0, 0, 30));
                        sc.Start();
                    }
                    catch { }
                }
                else
                {
                    sc.Start();
                }
                try
                {
                    sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 0, 30));
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                }
                sc.Refresh();
                bStarting = false;
            });
        }
        bool bStopping = false;
        public void StopService(Action<LogLevel, string> info)
        {
            if (bStopping)
            {
                info(LogLevel.LevInfo, "正在停止服务");
                return;
            }
            tf.StartNew(() =>
            {
                bStopping = true;
                if (sc == null)
                    sc = new ServiceController(SERVICENAME);
                sc.Refresh();
                if (sc.CanStop)
                {
                    try
                    {
                        sc.Stop();
                    }
                    catch { }
                }
                else
                {
                    var waitForStartThread = new Thread(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(1000);
                            sc.Refresh();
                            if (sc.CanStop)
                            {
                                try
                                {
                                    sc.Stop();
                                }
                                catch { }
                                break;
                            }
                        }
                    });
                    waitForStartThread.Start();
                }
                try
                {
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(50));
                }
                catch { }
                sc.Refresh();
                bStopping = false;
            });
        }

        /// <summary>
        /// 等待进程关闭
        /// </summary>
        /// <param name="ProcName"></param>
        /// <param name="timeout"></param>
        public static void WaitForProcessClosed(string ProcName, TimeSpan timeout)
        {
            Stopwatch Watch = new Stopwatch();
            Watch.Start();
            Func<bool> proc_exists = () =>
            {
                string tempName = "";
                int begpos;
                int endpos;
                foreach (Process thisProc in Process.GetProcesses())
                {
                    tempName = thisProc.ToString();
                    begpos = tempName.IndexOf("(") + 1;
                    endpos = tempName.IndexOf(")");
                    tempName = tempName.Substring(begpos, endpos - begpos);
                    if (tempName == ProcName)
                        return true;
                }
                return false;
            };
            while (proc_exists())
            {
                if (Watch.ElapsedMilliseconds >= timeout.TotalMilliseconds)
                {
                    CloseProcess(ProcName);
                    break;
                }
                Thread.Sleep(1000);
            }
            Watch.Stop();
            Watch = null;
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="ProcName">进程名称</param>
        /// <returns></returns>
        public static bool CloseProcess(string ProcName)
        {
            bool result = false;
            string tempName = "";
            int begpos;
            int endpos;
            foreach (Process thisProc in Process.GetProcesses())
            {
                tempName = thisProc.ToString();
                begpos = tempName.IndexOf("(") + 1;
                endpos = tempName.IndexOf(")");
                tempName = tempName.Substring(begpos, endpos - begpos);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                        thisProc.Kill(); // 当发送关闭窗口命令无效时强行结束进程
                    result = true;
                }
            }
            return result;
        }
        public string ServiceStatus
        {
            get
            {
                try
                {
                    if (sc == null)
                        sc = new ServiceController(SERVICENAME);
                    sc.Refresh();
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Stopped:
                            return "服务未运行";
                        case ServiceControllerStatus.StartPending:
                            return "服务正在启动";
                        case ServiceControllerStatus.StopPending:
                            return "服务正在停止";
                        case ServiceControllerStatus.Running:
                            return "服务正在运行";
                        case ServiceControllerStatus.ContinuePending:
                            return "服务即将继续";
                        case ServiceControllerStatus.PausePending:
                            return "服务即将暂停";
                        case ServiceControllerStatus.Paused:
                            return "服务已暂停";
                        default:
                            return "未知状态";
                    }

                }
                catch
                { return "服务已停止"; }
            }
        }

        public static bool CheckServicePath()
        {
            if (FileHelper.FileExist(AppDomain.CurrentDomain.BaseDirectory + "/Finance.exe")) {
                return true; 
            }
            return false;
        }
    }
}
