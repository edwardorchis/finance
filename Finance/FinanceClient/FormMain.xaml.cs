using Arthas.Controls.Metro;
using Arthas.Utility.Media;
using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.UI;
using Finance.UI;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace FinanceClient
{
    public partial class FormMain : MetroWindow
    {
        Brush m_brush = null;
        public FormMain()
        {
            InitializeComponent();
                        
            color1.ColorChange += delegate
            {
                // 不要通过XAML来绑定颜色，无法获取到通知
                BorderBrush = color1.CurrentColor.OpaqueSolidColorBrush;               
                ConfigHelper.SetConfigValue(APP_CONFIG_KEY_BORDERBRUSH, color1.CurrentColor.OpaqueSolidColorBrush.ToString());
            };
            var themeColor = ConfigHelper.GetConfigValue(APP_CONFIG_KEY_BORDERBRUSH);
            if (!string.IsNullOrEmpty(themeColor))
            {
                BrushConverter brushConverter = new BrushConverter();
                Brush brush = (Brush)brushConverter.ConvertFromString(themeColor);
                if (null != brush)
                {
                    BorderBrush = brush;
                }                    
            }
            this.Loaded += FormMain_Loaded;
            this.SizeChanged += FormMain_SizeChanged;

            DataFactory.Instance.HeartbeatTimeOutEvent += ()=> {                
                this.Dispatcher.Invoke(new Action(() =>
                {
                    m_brush = BorderBrush;
                    BorderBrush = HEARBEATTIMEOUT_BRUSH;
                }));
            };
            DataFactory.Instance.HeartbeatTimeOutRecoverEvent += () => {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (m_brush != null)
                        BorderBrush = m_brush;
                }));
            };
        }

        private void FormMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var kv in stackPanelMap)
            {
                var tabName = kv.Key.Replace(MENU_LIST_PREFIX, SUB_TAB_PREFIX);
                var tabCtrl= MainMenuTab.FindName(tabName) as MetroTabControl;
                foreach (MetroTabItem item in tabCtrl.Items)
                {
                    var frm = item.Content;
                    if (frm != null && frm is FinanceForm)
                    {
                        (frm as FinanceForm).OnSizeChanged(e);
                    }
                }
            }
        }
        Dictionary<string, StackPanel> stackPanelMap = new Dictionary<string, StackPanel>();
        private void FormMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = this.Title.Replace("Finance", ConfigurationManager.AppSettings["copyright"]);

            menuTableMap.ForEach(menu=> {
                StackPanel stackPanel = null;
                if (stackPanelMap.ContainsKey(MENU_LIST_PREFIX + menu.group))
                    stackPanel = stackPanelMap[MENU_LIST_PREFIX + menu.group];
                else
                {
                    stackPanel = MainMenuTab.FindName(MENU_LIST_PREFIX + menu.group) as StackPanel;
                    if (stackPanel != null)
                    {
                        stackPanelMap.Add(MENU_LIST_PREFIX + menu.group,stackPanel);
                    }
                }
                
                if (stackPanel != null)
                {                    
                    MetroExpander metroExpander = new MetroExpander();
                    metroExpander.Name = menu.name;
                    metroExpander.Header = menu.header;
                    MainMenuTab.RegisterName(menu.name, metroExpander);
                    stackPanel.Children.Add(metroExpander);
                }
            });

            foreach (var kv in stackPanelMap)
            {
                dalegateMenu(kv.Value);
            }
        }

        void dalegateMenu(StackPanel list_menu)
        {
            foreach (FrameworkElement fe in list_menu.Children)
            {
                if (fe is MetroExpander)
                {
                    var mexpander = fe as MetroExpander;
                    mexpander.Click += delegate (object sender, EventArgs e)
                    {
                        if (mexpander.CanHide && (mexpander.Children != null && mexpander.Children.Count > 0))
                        {
                            foreach (FrameworkElement fe1 in list_menu.Children)
                            {
                                if (fe1 is MetroExpander && fe1 != sender)
                                {
                                    (fe1 as MetroExpander).IsExpanded = false;
                                }
                            }
                        }
                        else
                        {
                            menuDispatch(fe.Name);
                        }                       
                    };
                }                
            }
        }

        void menuDispatch(string name)
        {
            try
            {
                switch (name)
                {
                    case "abount":
                        ShowForm("user_list");
                        break;                    
                    case "voucher_list":
                        var frm = new FormVoucherList();
                        frm.ShowSelectedItemEvent += (id) =>
                        {
                            var frmVoucher = FindForm("voucher_input") as FormVoucher;
                            if (frmVoucher == null)
                                frmVoucher = new FormVoucher();
                            frmVoucher.VoucherId = id;
                            ShowForm("凭证录入", "voucher_input", frmVoucher);
                        };
                        ShowForm("凭证列表",name,frm);
                        break;                    
                    case "settle":
                        MessageBoxResult mbr = FinanceMessageBox.Quest("确认现在结账吗？");
                        if (mbr == MessageBoxResult.Yes)
                        {
                            bool bEnd = false;
                            bool bTmp = false;
                            string message = "结账完成";
                            Task task = Task.Run(() =>
                            {
                                try
                                {
                                    DataFactory.Instance.GetAccountBalanceExecuter().Settle();
                                }
                                catch (Exception ex)
                                {
                                    message = ex.Message;
                                }
                                bEnd = true;
                            });
                            FinanceMessageBox.Progress("开始结账...", (args) =>
                            {
                                bTmp = !bTmp;
                                args.Message = "正在结账 " + (bTmp ? "..." : "");
                                args.Close = bEnd;
                            });
                            FinanceMessageBox.Info(message);


                        }
                        break;
                    default:
                        ShowForm(name);
                        break;
                }
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }

        }

        void ShowForm(string menu)
        {
            var item = menuTableMap.FirstOrDefault(m=>m.name == menu);
            if (null != item)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(FinanceAccountForm));
                var type = assembly.DefinedTypes.FirstOrDefault(typeInfo => typeInfo.Name == item.financeForm);
                if(null != type)
                {
                    object obj = Activator.CreateInstance(type);
                    ShowForm(item.header, item.name, obj as FinanceForm);
                }                
            }
        }

        MetroTabControl FindMetroTabControl(string menu)
        {
            MetroExpander me = MainMenuTab.FindName(menu) as MetroExpander;
            if (me == null)
                return null;
            var name = (me.Parent as FrameworkElement).Name;
            var tabName = name.Replace(MENU_LIST_PREFIX, SUB_TAB_PREFIX);
            return MainMenuTab.FindName(tabName) as MetroTabControl;
        }

        void ShowForm(string header, string name, FinanceForm form)
        {
            var tabControl = FindMetroTabControl(name);
            if(tabControl != null)
            {
                var mainTabItem = FindMainTabItem(tabControl);
                if (null != mainTabItem)
                {
                    mainTabItem.IsSelected = true;


                    var tabName = SUB_TAB_PREFIX + name;
                    var me = MainMenuTab.FindName(tabName) as MetroTabItem;
                    if (me != null)
                    {
                        me.IsSelected = true;
                        return;
                    }

                    if (form is FormUdefReport)
                    {
                        form.Name = name;
                        var frmUdfReport = form as FormUdefReport;
                        frmUdfReport.xTitle = header;
                        frmUdfReport.ShowSelectedItemEvent += (id) =>
                        {
                             var frmVoucher = FindForm("voucher_input") as FormVoucher;
                             if (frmVoucher == null)
                                 frmVoucher = new FormVoucher();
                             frmVoucher.VoucherId = id;
                             ShowForm("凭证录入", "voucher_input", frmVoucher);
                        };
                    }                    

                    MetroTabItem tabItem = new MetroTabItem();                    
                    tabItem.Header = header;                    
                    tabItem.Content = form;
                    tabItem.Name = tabName;
                    tabControl.Items.Add(tabItem);
                    MainMenuTab.RegisterName(tabName, tabItem);
                    tabItem.IsSelected = true;
                    tabItem.ButtonClick += TabItem_ButtonClick;
                }
            }
        }

        private void TabItem_ButtonClick(object sender, EventArgs e)
        {
            var metroTabItem = sender as MetroTabItem;
            if (metroTabItem == null)
                return;
            var frm = metroTabItem.Content as FinanceForm;
            if (frm != null)
            {   
                CancelEventArgs args = new CancelEventArgs();
                frm.Closing(args);
                if (args.Cancel)
                    return;
            }
            var tabName = metroTabItem.Name;
            var tabControl = FindMetroTabControl(tabName.Replace(SUB_TAB_PREFIX,""));
            if (tabControl == null)
                return;
            tabControl.Items.Remove(metroTabItem);
            tabControl.UnregisterName(tabName);
        }

        MetroMenuTabItem FindMainTabItem(MetroTabControl subTabControl)
        {
            var grid = subTabControl.Parent as FrameworkElement;
            if (grid != null)
            {
                var mainTabItem = grid.Parent as MetroMenuTabItem;
                return mainTabItem;
            }
            return null;
        }

        FinanceForm FindForm(string name)
        {
            var me = MainMenuTab.FindName("tab_" + name) as MetroTabItem;
            if (me != null)
            {
                me.IsSelected = true;
                return me.Content as FinanceForm;
            }
            
            return null;
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MetroMenuItem;
            if (menu == null)
                return;
            switch (menu.Name)
            {
                case "help":
                    Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = "https://www.cnblogs.com/edwardorchis/p/10506391.html";
                    proc.Start();
                    break;
                case "changePassword":
                    FormUserChangePasswordPopup frm = new FormUserChangePasswordPopup();
                    frm.ShowDialog();
                    break;
                case "new":
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo("FinanceAcountManager.exe");
                    process.StartInfo = startInfo;
                    process.Start();
                    break;
            }
           
        }
    }
}
