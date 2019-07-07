using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using static Finance.Account.UI.Model.Constant;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormVoucherList.xaml 的交互逻辑
    /// </summary>
    public partial class FormUdefReport : FinanceForm
    {
        public event ShowSelectedItemEventHandler ShowSelectedItemEvent;
        public FormUdefReport()
        {
            InitializeComponent();
            datagrid.MouseDoubleClick += Datagrid_MouseDoubleClick;
        }


        Dictionary<string, object> m_filter = null;
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;
                switch (txt)
                {
                    case "query":
                        var frmFilter = new FormUdefReportFilterPopup(this);
                        frmFilter.Filter = m_filter;
                        frmFilter.FilterPopupEvent += (args) =>
                        {
                            m_filter = args.Filter;
                            isShowed = false;
                            FinanceForm_Loaded(this, null);
                        };
                        frmFilter.Show();
                        break;
                    case "export":

                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        bool isShowed = false;
        List<UdefTemplateItem> mHeaderList = null;
        List<Dictionary<string, object>> mEntries = null;
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isShowed)
                    return;
                SetDefaultFilter();
                UdefReportDataSet dataSet = DataFactory.Instance.GetUdefReportExecuter().Query(Name, m_filter);
                var headerList = dataSet.header.OrderBy(h => h.tabIndex).ToList();
                mHeaderList = headerList;

                DataTable dt = new DataTable();
                datagrid.Columns.Clear();
                foreach (var header in headerList)
                {
                    if (header.dataType == "number")
                    {
                        var col = new DataGridTemplateColumn();
                        col.Width = header.width == 0 ? 200 : header.width;
                        col.Header = header.label;
                        //col.HeaderTemplate = GetDataTemplate(header.name, false);
                        col.CellTemplate = GetDataTemplate(header.name, true);
                        if (header.label == "ID")
                        {
                            col.Visibility = Visibility.Hidden;
                        }
                        datagrid.Columns.Add(col);
                    }
                    else
                    {
                        var col = new DataGridTextColumn { };
                        col.Binding = new Binding(header.name);
                        col.Width = header.width == 0 ? 200 : header.width;
                        col.Header = header.label;
                        if (header.label == "ID")
                        {
                            col.Visibility = Visibility.Hidden;
                        }
                        datagrid.Columns.Add(col);
                    }
                    dt.Columns.Add(header.name);
                }
                dataSet.entries.ForEach(data =>
                {
                    DataRow dr = dt.NewRow();
                    foreach (var kv in data)
                    {
                        if (dt.Columns.Contains(kv.Key))
                        {
                            dr[kv.Key] = kv.Value;
                        }
                    }
                    dt.Rows.Add(dr);
                });
                mEntries = dataSet.entries;
                datagrid.ItemsSource = dt.DefaultView;
                isShowed = true;
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }
        }

        private void SetDefaultFilter()
        {
            if (m_filter != null)
                return;
            var lst = DataFactory.Instance.GetTemplateExecuter().GetUdefTemplate(Name);
            if (lst == null)
                return;
            m_filter = new Dictionary<string, object>();
            foreach (var item in lst)
            {
                object val = item.defaultVal;
                var str = val.ToString();
                if (str.StartsWith("$") && str.IndexOf("(") != -1 && str.LastIndexOf(")") > 0)
                {
                    str = str.Substring(str.IndexOf("(") + 1, str.LastIndexOf(")") - str.IndexOf("(") - 1);
                    switch (str)
                    {
                        case "currentYear":
                            val = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentYear);
                            break;
                        case "currentPeriod":
                            val = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentPeriod);
                            break;
                    }
                }
                m_filter.Add(item.name, val);
            }
        }

        private DataTemplate GetDataTemplate(string bindName ,bool bCell)
        {
            XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XElement xTextBlock = new XElement(ns + "TextBlock",                
                new XAttribute("TextAlignment", "Right")
            );
            if (bCell)
            {
                xTextBlock.Add(new XAttribute("Text", "{Binding " + bindName + ",StringFormat={}{0:N}}"));
            }
            XElement xDataTemplate = new XElement(ns + "DataTemplate",
                new XAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")
            );
            xDataTemplate.Add(xTextBlock);
            XmlReader xr = xDataTemplate.CreateReader();
            DataTemplate dataTemplate = XamlReader.Load(xr) as DataTemplate;
            return dataTemplate;
        }


        private void Datagrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var item = datagrid.SelectedItem as DataRowView;
                if (item != null)
                {
                    int i = 0;
                    var bFinded = false;
                    for (i = 0; i < mHeaderList.Count; i++)
                    {
                        var header = mHeaderList[i];
                        if (header.label == "ID")
                        {
                            bFinded = true;
                            break;
                        }
                    }
                    if (bFinded)
                    {
                        var idStr = item.Row.ItemArray[i].ToString();
                        long id = 0;
                        if (long.TryParse(idStr, out id))
                            ShowSelectedItemEvent?.Invoke(id);
                    }                   
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message + "\r\n以数据集中作为ID列的数字找到凭证的，请检查存储过程是否正确。");
            }
        }


        public string xTitle
        {
            get { return (string)GetValue(xTitleProperty); }
            set { SetValue(xTitleProperty, value); }
        }

        static readonly DependencyProperty xTitleProperty =
            DependencyProperty.Register("xTitle", typeof(string), typeof(FormUdefReport), new PropertyMetadata("自定义报表"));


    }


}
