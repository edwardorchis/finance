using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormVoucherList.xaml 的交互逻辑
    /// </summary>
    public partial class FormCashflowSheet : FinanceForm
    {
        List<ExcelTemplateItem> m_excelTemlate = null;
        List<CashflowSheetItem> m_cashflowSheet = null;
        SheetModel _sheetModel = SheetModel.DEFAULT;
        private SheetModel SheetModel {
            set {
                _sheetModel = value;
                if (_sheetModel == SheetModel.DATA)
                {
                    datagrid.Visibility = Visibility.Visible;
                    gridTemplate.Visibility = Visibility.Hidden;
                    if (m_cashflowSheet == null)
                    {
                        var curYear = DataFactory.Instance.GetSystemProfileExecuter().GetString(SystemProfileKey.CurrentYear);
                        var curPeriod = DataFactory.Instance.GetSystemProfileExecuter().GetString(SystemProfileKey.CurrentPeriod);
                        var filter = new Dictionary<string, string>();
                        filter.Add("beginYear", curYear);
                        filter.Add("beginPeriod", curPeriod);
                        filter.Add("endYear", curYear);
                        filter.Add("endPeriod", curPeriod);
                        m_cashflowSheet = DataFactory.Instance.GetCashflowExecuter().ListSheet(filter);
                    }
                    datagrid.ItemsSource = m_cashflowSheet;
                }
                else if (_sheetModel == SheetModel.FORMULA)
                {
                    datagrid.Visibility = Visibility.Hidden;
                    gridTemplate.Visibility = Visibility.Visible;
                    if (m_excelTemlate == null)
                        m_excelTemlate = DataFactory.Instance.GetTemplateExecuter().GetExcelTemplate("现金流量表");
                    gridTemplate.ItemsSource = m_excelTemlate;
                }
            }
            get { return _sheetModel; }
        }

   


        public FormCashflowSheet()
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
                    case "formula":
                        if (SheetModel == SheetModel.FORMULA)
                            SheetModel = SheetModel.DATA;
                        else
                            SheetModel = SheetModel.FORMULA;
                            break;
                    case "export":
                        SaveFileDialog sflg = new SaveFileDialog();
                        sflg.Filter = "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx";
                        var bRnt = sflg.ShowDialog();
                        if (bRnt == null || bRnt == false)
                        {
                            return;
                        }
                        var curYear = DataFactory.Instance.GetSystemProfileExecuter().GetString(SystemProfileKey.CurrentYear);
                        var curPeriod = DataFactory.Instance.GetSystemProfileExecuter().GetString(SystemProfileKey.CurrentPeriod);
                        var filter = new Dictionary<string, string>();
                        filter.Add("beginYear", curYear);
                        filter.Add("beginPeriod", curPeriod);
                        filter.Add("endYear", curYear);
                        filter.Add("endPeriod", curPeriod);
                        DataFactory.Instance.GetCashflowExecuter().DownloadFile(sflg.FileName, filter);
                        FinanceMessageBox.Info("导出完成。");
                        break;
                    case "refresh":
                        m_excelTemlate = null;
                        m_cashflowSheet = null;
                        SheetModel = _sheetModel;
                        break;
                   
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
            SheetModel = SheetModel.DATA;
        }

        private void datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as CashflowSheetItem;
            if (drv == null)
                return;
            if (drv.Flag == 1)
                e.Row.Background = new SolidColorBrush(Colors.LightYellow);
            else
                e.Row.Background = new SolidColorBrush(Colors.White);
        }
    }


}
