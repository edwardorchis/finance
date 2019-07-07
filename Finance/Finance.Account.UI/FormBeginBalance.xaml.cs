using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormVoucherList.xaml 的交互逻辑
    /// </summary>
    public partial class FormBeginBalance : FinanceForm
    {       

        public FormBeginBalance()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;                
                switch (txt)
                {  
                    case "save":
                        Save();
                        FinanceMessageBox.Info("保存成功");
                        break;
                    case "finish":
                        Save();
                        DataFactory.Instance.GetBeginBalanceExecuter().Finish();
                        FinanceMessageBox.Info("结束初始化成功");
                        if (DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.IsInited) == 1)
                        {
                            datagrid.IsReadOnly = true;
                            save.IsEnabled = false;
                            finish.IsEnabled = false;
                        }
                        break;
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void Save()
        {
            List<BeginBalanceItem> items = datagrid.ItemsSource as List<Model.BeginBalanceItem>;
            var content = new List<BeginBalance>();
            items.ForEach(b => {
                if (b.CreditAmount != 0 || b.DebitsAmount != 0)
                {
                    var item = new BeginBalance();
                    item.accountSubjectId = b.Id;
                    item.creditAmount = b.CreditAmount;
                    item.debitsAmount = b.DebitsAmount;
                    content.Add(item);
                }
            });
            DataFactory.Instance.GetBeginBalanceExecuter().Save(content);
        }
        bool isShow = false;
        string mSpaceString = "                         ";
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (isShow)
                return;
            isShow = true;
            if (DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.IsInited) == 1)
            {
                datagrid.IsReadOnly = true;
                save.IsEnabled = false;
                finish.IsEnabled = false;
            }

            var lstAso=DataFactory.Instance.GetAccountSubjectExecuter().List();
            var lstBb = DataFactory.Instance.GetBeginBalanceExecuter().List();
            List<BeginBalanceItem> lstItemSource = new List<BeginBalanceItem>();
            lstAso.ForEach(aso=> {
                var item = new BeginBalanceItem();
                item.Id = aso.id;
                item.No = aso.no = mSpaceString.Substring(0, (aso.level -1) * 4) + aso.no;
                item.Name = aso.name;
                item.IsReadOnly = aso.isHasChild;
                var bb = lstBb.FirstOrDefault(b => b.accountSubjectId == aso.id);
                if (bb != null)
                {
                    item.DebitsAmount = bb.debitsAmount;
                    item.CreditAmount = bb.creditAmount;
                }
                else
                {
                    item.DebitsAmount = 0M;
                    item.CreditAmount = 0M;
                }
                lstItemSource.Add(item);
            });
            datagrid.ItemsSource = lstItemSource;
            CalcTotal();
        }

        void CalcTotal()
        {
            var lstItemSource = datagrid.ItemsSource as List<BeginBalanceItem>;
            List<BeginBalanceItem> totalItemSource = new List<Model.BeginBalanceItem>();
            var totalDebitsAmount = lstItemSource.Sum(b => b.DebitsAmount);
            var totalCreditAmount = lstItemSource.Sum(b => b.CreditAmount);
            totalItemSource.Clear();
            totalItemSource.Add(
                new BeginBalanceItem { Id = 0, No = "合计", Name = "", DebitsAmount = totalDebitsAmount, CreditAmount = totalCreditAmount }
                );
            datagridTotal.ItemsSource = totalItemSource;
        }
        private void datagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            CalcTotal();
        }
    }


}
