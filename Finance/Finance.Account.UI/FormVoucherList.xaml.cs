using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.SDK.Utils;
using Finance.Account.UI.Model;
using Finance.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.Account.UI.Model.Constant;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormVoucherList.xaml 的交互逻辑
    /// </summary>
    public partial class FormVoucherList : FinanceForm
    {
    
        public event ShowSelectedItemEventHandler ShowSelectedItemEvent;

        Dictionary<string, object> m_filter = null;

        public FormVoucherList()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;
                IDictionary<long, FinanceApiException> dictException = new Dictionary<long, FinanceApiException>();
                switch (txt)
                {
                    case "new":
                        ShowSelectedItemEvent?.Invoke(0);
                        break;
                    case "query":
                        FormVoucherListFilterPopup frmFilter = new FormVoucherListFilterPopup();
                        frmFilter.Filter = m_filter;
                        frmFilter.FilterPopupEvent += (args) =>
                        {
                            m_filter = args.Filter;
                            Refresh();
                        };
                        frmFilter.Show();
                        break;
                    case "delete":
                        DoActionSelectedItems((id)=> {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().Delete(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });                        
                        break;                  
                    case "check":
                        DoActionSelectedItems((id) => {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().Check(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });
                        break;
                    case "uncheck":
                        DoActionSelectedItems((id) => {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().UnCheck(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });
                        break;
                    case "cancel":
                        DoActionSelectedItems((id) => {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().Cancel(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });
                        break;
                    case "uncancel":
                        DoActionSelectedItems((id) => {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().UnCancel(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });
                        break;
                    case "post":
                        DoActionSelectedItems((id) => {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().Post(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });
                        break;
                    case "unpost":
                        DoActionSelectedItems((id) => {
                            try
                            {
                                DataFactory.Instance.GetVoucherExecuter().UnPost(id);
                            }
                            catch (FinanceApiException apiEx)
                            {
                                dictException.Add(id, apiEx);
                            }
                        });
                        break;
                }
                if (dictException.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<long, FinanceApiException> kv in dictException)
                    {
                        sb.AppendLine(string.Format("凭证【{0}】操作失败，{1}",GetVoucherWordNo(kv.Key),kv.Value.Message));
                    }
                    FinanceMessageBox.Error(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_filter == null)
            {
                m_filter = new Dictionary<string, object>();
                var year = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentYear);
                var period = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentPeriod);
                m_filter.Add("yearBegin", year);
                m_filter.Add("periodBegin", period);
                m_filter.Add("yearEnd", year);
                m_filter.Add("periodEnd", period);
            }
            Refresh();
        }

        void Refresh()
        {
            try
            {
                List<Voucher> lst = DataFactory.Instance.GetVoucherExecuter().List(m_filter);
                LoadList(lst);
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }
        }
 
        void LoadList(List<Voucher> list)
        {         
            List<VoucherListItem> voucherListItems = new List<VoucherListItem>();
            foreach (var voucher in list)
            {
                var header = voucher.header;
                var entries = voucher.entries;

                var isFistRow = true;
                foreach (var entry in entries)
                {
                    var item = new VoucherListItem();
                    item.id = header.id;

                    if (isFistRow)
                    {
                        var status = (VoucherStatus)header.status;
                        if (Constant.VoucherStatusDictionary.ContainsKey(status))
                        {
                            item.status = Constant.VoucherStatusDictionary[status];
                        }
                        item.date = header.date.ToString("yyyy-MM-dd");

                        item.period = string.Format("{0}.{1}", header.year, header.period);
                        item.wordno = string.Format("{0} - {1}", header.word, header.no);

                        //制单 出纳  过账人 经办
                        item.creater = DataFactory.Instance.GetUserExecuter().FindName(header.creater);
                        item.cashier = header.cashier;
                        item.poster = DataFactory.Instance.GetUserExecuter().FindName(header.poster);
                        item.agent = header.agent;
                        item.checker = DataFactory.Instance.GetUserExecuter().FindName(header.checker);

                        isFistRow = false;
                    }                    
                    item.explanation = entry.explanation;
                    var accountSubject = DataFactory.Instance.GetAccountSubjectExecuter().Find(entry.accountSubjectId);
                    item.subjectno = accountSubject.no;
                    item.subjectname = accountSubject.name;
                    item.linkno = entry.linkNo;

                    item.amount =  entry.amount.ToString("0.00");
                    if (entry.direction == 1)
                        item.debits = item.amount;
                    else
                        item.credit = item.amount;
                    voucherListItems.Add(item);
                }              
            }
            datagrid.ItemsSource = voucherListItems;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item=datagrid.SelectedItem as VoucherListItem;
            if(item !=null)
                ShowSelectedItemEvent?.Invoke(item.id);
            e.Handled = true;
        }

        void DoActionSelectedItems(Action<long> action)
        {
            var lst = datagrid.SelectedItems;
            if (lst != null)
            {
                List<long> ids = new List<long>();
                foreach(VoucherListItem item in lst)
                {
                    if (!ids.Contains(item.id))
                    {
                        ids.Add(item.id);
                        action?.Invoke(item.id);
                    }
                }               
                Refresh();
                SelectedVoucherId(ids.FirstOrDefault());
            }
        }

        string GetVoucherWordNo(long id)
        {
            var voucherListItems = datagrid.ItemsSource as List<VoucherListItem>;
            var item = voucherListItems.FirstOrDefault(v=>v.id==id);
            if (item == null)
                return "";
            return item.wordno;
        }

        private long GetSelectedVoucherId()
        {
            var item = datagrid.SelectedItem as VoucherListItem;
            if (item != null)
                return item.id;
            return 0;
        }

        private void SelectedVoucherId(long id)
        {
            var voucherListItems = datagrid.ItemsSource as List<VoucherListItem>;
            if (voucherListItems == null)
                return;
            var item = voucherListItems.FirstOrDefault(v => v.id == id);
            if (item != null)
            {
                datagrid.SelectedItem = item;
            }
        }
    }


}
