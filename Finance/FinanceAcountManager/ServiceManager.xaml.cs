using Finance.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace FinanceAcountManager
{
    /// <summary>
    /// AccoutManager.xaml 的交互逻辑
    /// </summary>
    public partial class ServiceManager : UserControl
    {
        static ILogger logger()
        {
            return Logger.GetLogger(typeof(ServiceManager));
        }
        public ServiceManager()
        {
            InitializeComponent();
        }

        public void logCallback(LogLevel level, string message)
        {
            if (LogLevel.LevDebug == level)
            {
                return;
            }

            if (LogLevel.LevWarn > level)
            {
                actManagerLoggerView.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else if (LogLevel.LevInfo == level || LogLevel.LevWarn == level)
            {
                actManagerLoggerView.Foreground = new SolidColorBrush(Color.FromRgb(34, 177, 76));
            }
            else
            {
                actManagerLoggerView.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            actManagerLoggerView.Text = message;
        }

        System.Timers.Timer _statusRefreshTimer = null;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.HookLogger(logCallback);
            _statusRefreshTimer = new System.Timers.Timer();
            _statusRefreshTimer.Interval = 1000;
            _statusRefreshTimer.AutoReset = true;
            _statusRefreshTimer.Elapsed += _statusRefreshTimer_Tick;
            _statusRefreshTimer.Start();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string btnName = (sender as Button).Name;
                switch (btnName)
                {
                    case "start":
                        ServiceHelper.Instance.StartService(logCallback);
                        break;
                    case "stop":
                        ServiceHelper.Instance.StopService(logCallback);
                        break;
                }
            }
            catch (Exception ex)
            {
                logCallback(LogLevel.LevError, ex.Message);
            }           
        }

        

        private void _statusRefreshTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            showMessage(ServiceHelper.Instance.ServiceStatus);
        }

        private void showMessage(string msg)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                actManagerLoggerView.Text = msg;
            }));
        }
    }
}
