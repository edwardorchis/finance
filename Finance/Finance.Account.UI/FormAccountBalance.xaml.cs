using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.UI;
using Finance.Utils;
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
    public partial class FormAccountBalance : FinanceForm
    {       

        public FormAccountBalance()
        {
            InitializeComponent();
        }
        IDictionary<string, object> m_filter = null;
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;                
                switch (txt)
                {
                    case "query":
                        var frmFilter = new FormListFilterPopup();
                        frmFilter.Filter = m_filter;
                        frmFilter.FilterPopupEvent += (args) =>
                        {
                            m_filter = args.Filter;
                            Query();
                        };
                        frmFilter.Show();
                        break;
                    case "export":
                        break;
                    case "refresh":
                        Query();
                        break;
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void Query()
        {
            var lstAso = DataFactory.Instance.GetAccountSubjectExecuter().List();
            var package = DataFactory.Instance.GetAccountBalanceExecuter().Query(
                CommonUtils.TryParseInt(m_filter["beginYear"].ToString()),
                CommonUtils.TryParseInt(m_filter["beginPeriod"].ToString()),
                CommonUtils.TryParseInt(m_filter["endYear"].ToString()),
                CommonUtils.TryParseInt(m_filter["endPeriod"].ToString())
            );

            var lstBegin = package.ListBeginBalnace;
            var lstOccurs = package.ListCurrentOccurs;

            List<AccountBalanceItem> lstItemSource = new List<AccountBalanceItem>();
            lstAso.ForEach(aso => {
                var item = new AccountBalanceItem();
                item.Id = aso.id;
                item.No = aso.no;
                item.Name = aso.name;
                if (lstBegin != null)
                {
                    item.begin = lstBegin.FirstOrDefault(b => b.accountSubjectId == aso.id);
                }
                if (lstOccurs != null)
                {
                    item.current = lstOccurs.FirstOrDefault(b => b.accountSubjectId == aso.id);
                }
                if (item.begin == null)
                    item.begin = new AccountAmountItem();
                if (item.current == null)
                    item.current = new AccountAmountItem();

                item.end = new AccountAmountItem
                {
                    accountSubjectId = aso.id,
                    debitsAmount = item.begin.debitsAmount + item.current.debitsAmount,
                    creditAmount = item.begin.creditAmount + item.current.creditAmount
                };
                if (aso.rootId > 0)
                {
                    var rootItem = lstItemSource.FirstOrDefault(a => a.Id == aso.rootId);
                    if (rootItem != null)
                    {
                        rootItem.begin.creditAmount += item.begin.creditAmount;
                        rootItem.begin.debitsAmount += item.begin.debitsAmount;
                        rootItem.current.creditAmount += item.current.creditAmount;
                        rootItem.current.debitsAmount += item.current.debitsAmount;
                        rootItem.end.creditAmount += item.end.creditAmount;
                        rootItem.end.debitsAmount += item.end.debitsAmount;
                    }
                }
                else
                {
                    lstItemSource.Add(item);
                }
                
            });
            datagrid.ItemsSource = lstItemSource;
            CalcTotal();

            comment.Text = string.Format("{0} 年度 {1} 期间 到   {0} 年度 {1} 期间",
                        m_filter["beginYear"], m_filter["beginPeriod"], m_filter["endYear"], m_filter["endPeriod"]);
        }
        
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            var year = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentYear);
            var period = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentPeriod);

            var now = DateTime.Now;
            if (now.Year == year && now.Month == period)
            { }
            else
                now = CommonUtils.CalcMaxPeriodDate(year, period);

            if (m_filter == null)
            {
                m_filter = new Dictionary<string, object>();
                m_filter.Add("beginYear", now.Year);
                m_filter.Add("beginPeriod", now.Month);
                m_filter.Add("endYear", now.Year);
                m_filter.Add("endPeriod", now.Month);
            }

            try
            { 
                Query();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void CalcTotal()
        {
            var lstItemSource = datagrid.ItemsSource as List<AccountBalanceItem>;
            List<AccountBalanceItem> totalItemSource = new List<AccountBalanceItem>();
            AccountAmountItem begin = new AccountAmountItem {
                debitsAmount = lstItemSource.Sum(b=>b.begin.debitsAmount),
                creditAmount = lstItemSource.Sum(b => b.begin.creditAmount),
            };
            AccountAmountItem current = new AccountAmountItem
            {
                debitsAmount = lstItemSource.Sum(b => b.current.debitsAmount),
                creditAmount = lstItemSource.Sum(b => b.current.creditAmount),
            };
            AccountAmountItem end = new AccountAmountItem
            {
                debitsAmount = lstItemSource.Sum(b => b.end.debitsAmount),
                creditAmount = lstItemSource.Sum(b => b.end.creditAmount),
            };
            totalItemSource.Clear();
            totalItemSource.Add(
                new AccountBalanceItem { Id = 0, No = "合计", Name = "",begin= begin, current= current, end= end }
                );
            datagridTotal.ItemsSource = totalItemSource;
        }       
    }


}
