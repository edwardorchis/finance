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
    internal partial class FormVoucherListFilterPopup : Window
    {
        public FilterPopupEventHandler FilterPopupEvent;

        public Dictionary<string,object> Filter { set; get; }
        public FormVoucherListFilterPopup()
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
            dictFilter.Add("yearBegin", TryParseInt(cmbYearBegin.Text));
            dictFilter.Add("periodBegin", TryParseInt(cmbPeriodBegin.Text));
            dictFilter.Add("yearEnd", TryParseInt(cmbYearEnd.Text));
            dictFilter.Add("periodEnd", TryParseInt(cmbPeriodEnd.Text));
            dictFilter.Add("dateBeign", dateBegin.Text);
            dictFilter.Add("dateEnd", dateEnd.Text);

            dictFilter.Add("normal", (bool)chkNormal.IsChecked ? 1 : 0);
            dictFilter.Add("checked", (bool)chkChecked.IsChecked ? 1 : 0);
            dictFilter.Add("canceled", (bool)chkCanceled.IsChecked ? 1 : 0);
            dictFilter.Add("posted", (bool)chkPosted.IsChecked ? 1 : 0);
            dictFilter.Add("settled", (bool)chkSettled.IsChecked ? 1 : 0);

            dictFilter.Add("creater", cmbCreater.SelectedValue);
            dictFilter.Add("checker", cmbChecker.SelectedValue);
            dictFilter.Add("poster", cmbPoster.SelectedValue);

            dictFilter.Add("cashier", txtCashier.Text);
            dictFilter.Add("agent", txtAgent.Text);
            dictFilter.Add("reference", txtReference.Text);
            dictFilter.Add("word", cmbWord.Text);

            dictFilter.Add("accountSubjectId", asbAccountSubject.Id);
            dictFilter.Add("explanation", txtExplanation.Text);
           
            return dictFilter;
        }

        void LoadFilter(IDictionary<string, object> filter)
        {
            if (filter == null)
                return;

            loadDateFilter(filter);
            loadStatusFilter(filter);
            loadUserFilter(filter);
            loadTextFilter(filter);
            loadEntryFilter(filter);
        }

        void loadDateFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("yearBegin") && filter.ContainsKey("periodBegin"))
            {
                int year = 0, period = 0;
                int.TryParse(filter["yearBegin"].ToString(), out year);
                int.TryParse(filter["periodBegin"].ToString(), out period);
                if (year > 0 && period > 0)
                {
                    cmbYearBegin.SelectedItem = year;
                    cmbPeriodBegin.SelectedItem = period;
                }
            }

            if (filter.ContainsKey("yearEnd") && filter.ContainsKey("periodEnd"))
            {
                int year = 0, period = 0;
                int.TryParse(filter["yearEnd"].ToString(), out year);
                int.TryParse(filter["periodEnd"].ToString(), out period);
                if (year > 0 && period > 0)
                {
                    cmbYearEnd.SelectedItem = year;
                    cmbPeriodEnd.SelectedItem = period;
                }
            }

            if (filter.ContainsKey("dateBeign"))
            {
                DateTime date = new DateTime();
                var b = DateTime.TryParse(filter["dateBeign"].ToString(), out date);
                if (b)
                {
                    dateBegin.SelectedDate = date;
                }
            }

            if (filter.ContainsKey("dateEnd"))
            {
                DateTime date = new DateTime();
                var b = DateTime.TryParse(filter["dateEnd"].ToString(), out date);
                if (b)
                {
                    dateEnd.SelectedDate = date;
                }
            }
        }

        void loadStatusFilter(IDictionary<string, object> filter)
        {
            bool bNormal = false, bChecked = false, bCanceled = false, bPosted = false, bSettled = false;
            if (filter.ContainsKey("normal"))
            {
                int result = 0;
                int.TryParse(filter["normal"].ToString(), out result);
                bNormal = result == 1;
            }
            if (filter.ContainsKey("checked"))
            {
                int result = 0;
                int.TryParse(filter["checked"].ToString(), out result);
                bChecked = result == 1;
            }
            if (filter.ContainsKey("canceled"))
            {
                int result = 0;
                int.TryParse(filter["canceled"].ToString(), out result);
                bCanceled = result == 1;
            }
            if (filter.ContainsKey("posted"))
            {
                int result = 0;
                int.TryParse(filter["posted"].ToString(), out result);
                bPosted = result == 1;
            }
            if (filter.ContainsKey("settled"))
            {
                int result = 0;
                int.TryParse(filter["settled"].ToString(), out result);
                bSettled = result == 1;
            }
            var str = string.Empty;

            if (bNormal)
            {
                chkNormal.IsChecked = true;
            }
            if (bChecked)
            {
                chkChecked.IsChecked = true;
            }
            if (bCanceled)
            {
                chkCanceled.IsChecked = true;
            }
            if (bPosted)
            {
                chkPosted.IsChecked = true;
            }
            if (bSettled)
            {
                chkSettled.IsChecked = true;
            }
            
        }

        void loadUserFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("creater") && filter["creater"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["creater"].ToString(), out tmp);
                if (tmp > 0)
                {
                    cmbCreater.SelectedValue = tmp;
                }
            }
            if (filter.ContainsKey("checker") && filter["checker"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["checker"].ToString(), out tmp);
                if (tmp > 0)
                {
                    cmbChecker.SelectedValue = tmp;
                }
            }
            if (filter.ContainsKey("poster") && filter["poster"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["poster"].ToString(), out tmp);
                if (tmp > 0)
                {
                    cmbPoster.SelectedValue = tmp;
                }
            }
        }

        void loadTextFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("cashier") && filter["cashier"] != null)
            {
                var tmp = filter["cashier"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    txtCashier.Text = tmp;
                }
            }
            if (filter.ContainsKey("agent") && filter["agent"] != null)
            {
                var tmp = filter["agent"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    txtAgent.Text = tmp;
                }
            }
            if (filter.ContainsKey("reference") && filter["reference"] != null)
            {
                var tmp = filter["reference"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    txtReference.Text = tmp;
                }
            }
            if (filter.ContainsKey("word") && filter["word"] != null)
            {
                var tmp = filter["word"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    cmbWord.SelectedValue = tmp;
                }
            }
        }

        void loadEntryFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("explanation") && filter["explanation"] != null)
            {
                var tmp = filter["explanation"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    txtExplanation.Text = tmp;
                }
            }

            if (filter.ContainsKey("accountSubjectId") && filter["accountSubjectId"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["accountSubjectId"].ToString(), out tmp);
                if (tmp > 0)
                {
                    asbAccountSubject.Id = tmp;
                }
            }
            
        }



        int TryParseInt(string str)
        {
            int result = 0;
            int.TryParse(str, out result);
            return result;
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

            cmbWord.ItemsSource = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.ProofOfWords).Select(a=>a.name);

            var users= DataFactory.Instance.GetUserExecuter().List();
            cmbCreater.ItemsSource = users;
            cmbChecker.ItemsSource = users;
            cmbPoster.ItemsSource = users;

            var year = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentYear);
            var period = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentPeriod);

            var now = DateTime.Now;
            if (now.Year == year && now.Month == period)
            { }
            else
                now = CommonUtils.CalcMaxPeriodDate(year, period);

            cmbYearBegin.SelectedValue = now.Year;
            cmbYearEnd.SelectedValue = now.Year;

            cmbPeriodBegin.SelectedValue = now.Month;
            cmbPeriodEnd.SelectedValue = now.Month;

            //dateBegin.Text = new DateTime(now.Year,now.Month,1).ToString("yyyy-MM-dd");
            //dateEnd.Text = now.ToString("yyyy-MM-dd");

            if (Filter != null)
                LoadFilter(Filter);
        }
    }
}
