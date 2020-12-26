using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.UI;
using Finance.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormVoucherList.xaml 的交互逻辑
    /// </summary>
    public partial class FormBalanceSheet : FinanceForm
    {
        IBalanceSheetExecuter executer= DataFactory.Instance.GetBalanceSheetExecuter();
        List<ExcelTemplateItem> m_lstTemplate = null;
        IDictionary<string, object> m_filter = null;
        public FormBalanceSheet()
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
                    case "query":
                        var frmFilter = new FormListFilterPopup();
                        frmFilter.Filter = m_filter;
                        frmFilter.FilterPopupEvent += (args) =>
                        {
                            m_filter = args.Filter;
                            SheetModel = SheetModel;
                        };
                        frmFilter.Show();
                        break;                    
                    case "refresh":
                        SheetModel = SheetModel;
                        break;
                    case "formula":
                        if (SheetModel == SheetModel.FORMULA)
                            SheetModel = SheetModel.DATA;
                        else
                            SheetModel = SheetModel.FORMULA;
                        break;
                    case "exportformula":
                        SaveFileDialog sflg = new SaveFileDialog();
                        sflg.Filter = "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx";
                        sflg.FileName = "资产负债表";
                        var bRnt = sflg.ShowDialog();
                        if (bRnt == null || bRnt == false)
                        {
                            return;
                        }
                        ExcelExportor exportor = new ExcelExportor(new BalanceSheetExportHandler());
                        var dt = EntityConvertor<ExcelTemplateItem>.ToDataTable(m_lstTemplate);

                        MemoryStream ms = new MemoryStream();
                        string flg = FileHelper.FileSuffix(sflg.FileName);
                        exportor.Export(ms, dt, flg);
                        using (FileStream fs = new FileStream(sflg.FileName, FileMode.Create, FileAccess.Write))
                        {
                            byte[] data = ms.ToArray();
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }
                        ms.Close();
                        ms.Dispose();
                        FileHelper.ExplorePath(sflg.FileName.Substring(0, sflg.FileName.LastIndexOf("\\")));
                        break;
                    case "importformula":
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx";
                        ofd.Title = "选择文件";
                        ofd.RestoreDirectory = true;
                        if (ofd.ShowDialog() == true)
                        {
                            DataFactory.Instance.GetTemplateExecuter().UploadTemplate("BalanceSheet", ofd.FileName);
                            FinanceMessageBox.Info("导入成功");
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

        SheetModel _sheetModel = SheetModel.DEFAULT;
        private SheetModel SheetModel
        {
            set
            {
                _sheetModel = value;
                if (_sheetModel == SheetModel.DATA)
                {
                    Calc();
                    comment.Text = string.Format("{0} 年度 {1} 期间 到   {0} 年度 {1} 期间",
                        m_filter["beginYear"], m_filter["beginPeriod"], m_filter["endYear"], m_filter["endPeriod"]);
                }
                else if (_sheetModel == SheetModel.FORMULA)
                {
                    datagrid.ItemsSource = m_lstTemplate;
                    comment.Text = "";
                }
            }
            get { return _sheetModel; }
        }

        void Calc()
        {
            if (m_lstTemplate == null)
                throw new Exception("计算模板为空");
           
            var origin = new Dictionary<string, string>();
            foreach (var item in m_lstTemplate)
            {
                var lineNoJ = item.b;
                if (!string.IsNullOrEmpty(lineNoJ))
                {
                    origin.Add(lineNoJ + "y", item.c);
                    origin.Add(lineNoJ + "c", item.d);
                }
                var lineNoD = item.f;
                if (!string.IsNullOrEmpty(lineNoD))
                {
                    origin.Add(lineNoD + "y", item.g);
                    origin.Add(lineNoD + "c", item.h);
                }
            }

            var dest = executer.Calc(m_filter,origin);
            if (dest == null)
                throw new Exception("获取结果为空");

            List<ExcelTemplateItem> lstResorce = JsonConvert.DeserializeObject<List<ExcelTemplateItem>>(JsonConvert.SerializeObject(m_lstTemplate));
            foreach (KeyValuePair<string, string> kv in dest)
            {
                var key = kv.Key;
                var lineNo = key.Substring(0, key.Length - 1);
                var colkey = key.Substring(key.Length - 1);
                var item = lstResorce.FirstOrDefault(t=>t.b==lineNo || t.f==lineNo);
                if (item == null)
                    continue;
                if (item.b == lineNo)
                {
                    if (colkey == "y")
                    {
                        item.c = kv.Value;
                    }
                    else if (colkey == "c")
                    {
                        item.d = kv.Value;
                    }
                }
                else if (item.f == lineNo)
                {
                    if (colkey == "y")
                    {
                        item.g = kv.Value;
                    }
                    else if (colkey == "c")
                    {
                        item.h = kv.Value;
                    }
                }
            }
            datagrid.ItemsSource = lstResorce;
        }


        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            if(m_lstTemplate==null)
                m_lstTemplate = DataFactory.Instance.GetTemplateExecuter().GetExcelTemplate("资产负债表");

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
            
            SheetModel = SheetModel.DATA;  
        }

 
    }
    class BalanceSheetExportHandler : IExportHandler
    {
        public void Encode(ref DataTable data)
        {
            data.Columns["_a"].Caption = "资产";
            data.Columns["_b"].Caption = "行次";
            data.Columns["_c"].Caption = "期末余额";
            data.Columns["_d"].Caption = "年初余额";
            data.Columns["_e"].Caption = "负债和股东权益";
            data.Columns["_f"].Caption = "行次";
            data.Columns["_g"].Caption = "期末余额";
            data.Columns["_h"].Caption = "年初余额";
        }
    }

}
