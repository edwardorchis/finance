using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.UI;
using Finance.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FinanceClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void menu_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var menu = (sender as Button).Name;
                switch (menu)
                {
                    case "auxiliary_form":
                        AuxiliaryPopup popup = new AuxiliaryPopup(Finance.Account.Controls.Commons.AuxiliaryType.Invalid);
                        popup.SelectedEvent += (auxiliaryObj) =>
                        {
                            FinanceMessageBox.Info(auxiliaryObj.name);
                        };
                        popup.Show();
                        break;
                    case "user_list":
                        ShowForm("用户列表", "FormUser", new FormUser());
                        break;
                    case "account_subject":
                        ShowForm("科目", "FormAccountSubject", new FormAccountSubject());
                        break;
                    case "begin_balance":
                        ShowForm("初始余额表", "FormBeginBalance", new FormBeginBalance());
                        break;
                    case "voucher_input":
                        ShowForm("凭证录入", "FormVoucher", new FormVoucher());
                        break;
                    case "voucher_list":
                        var frm = new FormVoucherList();
                        frm.ShowSelectedItemEvent += (id) =>
                        {
                            var frmVoucher = FindForm("FormVoucher") as FormVoucher;
                            if (frmVoucher == null)
                            {
                                frmVoucher = new FormVoucher();
                                ShowForm("凭证录入", "FormVoucher", frmVoucher);
                            }
                            frmVoucher.VoucherId = id;
                        };
                        ShowForm("凭证列表", "FormVoucherList", frm);
                        break;
                    case "account_balance":
                        ShowForm("科目余额表", "FormAccountBalance", new FormAccountBalance());
                        break;
                    case "cashflow_sheet":
                        ShowForm("现金流量表", "FormCashflowSheet", new FormCashflowSheet());
                        break;
                    case "balance_sheet":
                        ShowForm("资产负债表", "FormBalanceSheet", new FormBalanceSheet());
                        break;
                    case "profit_sheet":
                        ShowForm("利润表", "FormProfitSheet", new FormProfitSheet());
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
                }
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }
        }

        FinanceForm FindForm(string name)
        {
            foreach (TabItem item in MainTab.Items)
            {
                var frm = item.Content as FinanceForm;
                if (frm == null)
                    continue;
                if (frm.Name.ToString() == name)
                {
                    item.IsSelected = true;
                    return frm;
                }
            }
            return null;
        }

        void ShowForm(string header,string name,FinanceForm form)
        {
            foreach (TabItem item in MainTab.Items)
            {
                if ((item.Content as Control).Name.ToString() == name)
                {
                    item.IsSelected = true;
                    return;
                }
            }

            TabItem tabItem = new TabItem();
            tabItem.Header = header;
            tabItem.SetResourceReference(StyleProperty, "tabItemStyle");
            form.Name = name;
            tabItem.Content = form;
            MainTab.Items.Add(tabItem);
            tabItem.IsSelected = true;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Control;
            var tabItem= item.TemplatedParent as TabItem;
            var content = tabItem.Content as FinanceForm;
            Console.WriteLine(content.Name);

            CancelEventArgs args = new CancelEventArgs();
            content.Closing(args);
            if (args.Cancel)
                return;
            MainTab.Items.Remove(tabItem);

            e.Handled = true;
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            foreach (TabItem item in MainTab.Items)
            {
                CancelEventArgs args = new CancelEventArgs();
                var frm=item.Content as FinanceForm;
                if (frm != null)
                {
                    frm.Closing(args);
                    if (args.Cancel)
                    {
                        item.IsSelected = true;
                        return;
                    }
                }               
            }
        }
    }
}
