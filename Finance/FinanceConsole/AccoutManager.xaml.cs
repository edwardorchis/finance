using Arthas.Controls.Metro;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceConsole
{
    /// <summary>
    /// AccoutManager.xaml 的交互逻辑
    /// </summary>
    public partial class AccoutManager : UserControl
    {
        public AccoutManager()
        {
            InitializeComponent();
        }

        int m_FocusRowIndex = -1;

        private void btnOperate_Initialized(object sender, EventArgs e)
        {
            //设置右键菜单为null 
            (sender as Button).ContextMenu = null;
        }
        private void btnOperate_Click(object sender, RoutedEventArgs e)
        {
            //目标
            this.contextMenu.PlacementTarget = sender as Button;
            //位置
            this.contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单
            this.contextMenu.IsOpen = true;

            string[] tmp = (sender as Button).Name.Split('_');
            if (tmp.Count() < 2)
            {
                return;
            }
            m_FocusRowIndex = int.Parse(tmp[1]);
        }

        private void operate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                actManagerLoggerView.Text = "开始执行...";
                Logger.RestLogger();
                Logger.HookLogger(logCallback);

                MenuItem menuItem = sender as MenuItem;
                TextBox txtNo = grid.FindName("txtNo_" + m_FocusRowIndex) as TextBox;
                string actNo = txtNo.Text;
                if ("init_k" == menuItem.Name)
                {
                    CommondHandler.Process("act.init " + actNo + " -k -f");
                }
                else
                {
                    CommondHandler.Process("act." + menuItem.Name + " " + actNo + " -f");
                }

                UserControl_Loaded(null, null);
            }
            catch (Exception ex)
            {
                logCallback(LogLevel.LevError, ex.Message);
            }
            finally
            {                
                Logger.RestLogger();
            }
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

        void releaseCell(string pref, int i)
        {
            string name = pref + i;
            object ctl = grid.FindName(name);
            if (ctl == null)
            {
                return;
            }
            Control c = (Control)(ctl);
            grid.Children.Remove(c);
            grid.UnregisterName(name);
        }

        void clearRows()
        {
            if (grid.RowDefinitions.Count > 0)
            {
                
                for (int i = 0; i < grid.RowDefinitions.Count; ++i)
                {
                    releaseCell("txtName_" , i);
                    releaseCell("txtNo_", i);
                    releaseCell("btnOprate_", i);
                }
                grid.RowDefinitions.RemoveRange(0, grid.RowDefinitions.Count);
            }
        }

        public void actPrintCallback(LogLevel level, string message)
        {
            if (LogLevel.LevInfo != level)
            {
                return;
            }

            clearRows();
            List<string> lines = Regex.Split(message, "\r\n", RegexOptions.IgnoreCase).ToList();

            int index = 0;
            foreach (string line in lines)
            {
                List<string> cols = line.Split('\t').ToList();
                if (cols.Count != 6)
                {
                    continue;
                }

                if ("id|" == cols[0])
                {
                    continue;
                }

                var row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                TextBox txtName = new TextBox();
                txtName.Text = cols[2].TrimEnd('|');
                txtName.Name = "txtName_" + index;
                grid.RegisterName("txtName_" + index, txtName);
                grid.Children.Add(txtName);
                Grid.SetRow(txtName, index);
                Grid.SetColumn(txtName, 0);

                TextBox txtNo = new TextBox();
                txtNo.Text = cols[1].TrimEnd('|');
                txtNo.Name = "txtNo_" + index;
                grid.RegisterName("txtNo_" + index, txtNo);
                grid.Children.Add(txtNo);
                Grid.SetRow(txtNo, index);
                Grid.SetColumn(txtNo, 1);

                MetroButton btnOprate = new MetroButton();
                btnOprate.Content = "更多";
                btnOprate.Name = "btnOprate_" + index;
                btnOprate.Initialized += btnOperate_Initialized;
                btnOprate.Click += btnOperate_Click;
                grid.RegisterName("btnOprate_" + index, btnOprate);
                grid.Children.Add(btnOprate);
                Grid.SetRow(btnOprate, index);
                Grid.SetColumn(btnOprate, 2);

                index++;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            clearRows();
            Logger.RestLogger();
            Logger.HookLogger(actPrintCallback);

            CommondHandler.Process("act.print");
            Logger.RestLogger();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {          
            try
            {
                actManagerLoggerView.Text = "开始执行...";
                Logger.RestLogger();
                Logger.HookLogger(logCallback);

                string btnName = (sender as Button).Name;
                FormAccountManagerPopup popup = new FormAccountManagerPopup();
                popup.FinanceMsgEvent = (msgType, paras) => {
                    bool bRet = false;
                    try
                    {
                        int iRet = CommondHandler.Process("act." + btnName + " "
                            + paras["no"] + " "
                            + paras["name"] + " "
                            + "-f");
                        if (0 == iRet)
                        {
                            UserControl_Loaded(null, null);
                            bRet = true;
                        }
                    }
                    catch(Exception fex)
                    {
                        logCallback(LogLevel.LevError, fex.Message);                        
                    }
                    finally
                    {
                        Logger.RestLogger();
                    }
                    return bRet;
                };
                popup.ShowDialog();
            }
            catch (Exception ex)
            {
                logCallback(LogLevel.LevError, ex.Message);
            }
            finally
            {
                Logger.RestLogger();
            }
        }
    }
}
