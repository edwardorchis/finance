using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Finance.Account.UI.Model.Constant;

namespace Finance.Account.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    internal partial class FormListFilterPopup : Window
    {
        public FilterPopupEventHandler FilterPopupEvent;

        public IDictionary<string, object> Filter { set; get; }
        public FormListFilterPopup()
        {
            InitializeComponent();           
        }     

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (FilterPopupEvent != null)
            {
                FilterPopupEventArgs args = new FilterPopupEventArgs { Filter = ReadFilter()};
                try
                {
                    FilterPopupEvent(args);
                }
                catch (Exception ex)
                {
                    FinanceMessageBox.Error(ex.Message);
                }
            }
            Close();
        }

        Dictionary<string, object> ReadFilter()
        {
            Dictionary<string, object> dictFilter = new Dictionary<string, object>();
            dictFilter.Add("beginYear", CommonUtils.TryParseInt( cmbYearBegin.Text));
            dictFilter.Add("beginPeriod", CommonUtils.TryParseInt(cmbPeriodBegin.Text));
            dictFilter.Add("endYear", CommonUtils.TryParseInt(cmbYearEnd.Text));
            dictFilter.Add("endPeriod", CommonUtils.TryParseInt(cmbPeriodEnd.Text));
           
            return dictFilter;
        }

        void LoadFilter(IDictionary<string, object> filter)
        {
            if (filter == null)
                return;

            loadDateFilter(filter);
        }

        void loadDateFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("beginYear") && filter.ContainsKey("beginPeriod"))
            {
                int year = 0, period = 0;
                int.TryParse(filter["beginYear"].ToString(), out year);
                int.TryParse(filter["beginPeriod"].ToString(), out period);
                if (year > 0 && period > 0)
                {
                    cmbYearBegin.SelectedItem = year;
                    cmbPeriodBegin.SelectedItem = period;
                }
            }

            if (filter.ContainsKey("endYear") && filter.ContainsKey("endPeriod"))
            {
                int year = 0, period = 0;
                int.TryParse(filter["endYear"].ToString(), out year);
                int.TryParse(filter["endPeriod"].ToString(), out period);
                if (year > 0 && period > 0)
                {
                    cmbYearEnd.SelectedItem = year;
                    cmbPeriodEnd.SelectedItem = period;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> lstYear = new List<int>();
            List<int> lstPeriod = new List<int>();

            int i = 1990;
            while (i < 2100)
            {
                lstYear.Add(++i);
            }
            i = 1;
            while (i < 13)
            {
                lstPeriod.Add(++i);
            }

            cmbYearBegin.ItemsSource = lstYear;
            cmbYearEnd.ItemsSource = lstYear;

            cmbPeriodBegin.ItemsSource = lstPeriod;
            cmbPeriodEnd.ItemsSource = lstPeriod;
            
            if (Filter != null)
                LoadFilter(Filter);
        }
    }
}
