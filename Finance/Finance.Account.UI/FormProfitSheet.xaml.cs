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
    public partial class FormProfitSheet : FinanceForm
    {
        IProfitSheetExecuter executer= DataFactory.Instance.GetProfitSheetExecuter();
        List<ExcelTemplateItem> m_lstTemplate = null;
        public FormProfitSheet()
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
                        frmFilter.Hide2();
                        frmFilter.FilterPopupEvent += (args) =>
                        {
                            m_filter = args.Filter;
                            m_lstTemplate = DataFactory.Instance.GetTemplateExecuter().GetExcelTemplate("利润表");
                            SheetModel = SheetModel;
                        };
                        frmFilter.Show();
                        break;
                    case "export":

                        break;
                    case "refresh":
                        m_lstTemplate = DataFactory.Instance.GetTemplateExecuter().GetExcelTemplate("利润表");
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
                        sflg.FileName = "利润表";
                        var bRnt = sflg.ShowDialog();
                        if (bRnt == null || bRnt == false)
                        {
                            return;
                        }
                        ExcelExportor exportor = new ExcelExportor(new ProfitSheetExportHandler());
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
                            DataFactory.Instance.GetTemplateExecuter().UploadTemplate("ProfitSheet", ofd.FileName);
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
                }
                else if (_sheetModel == SheetModel.FORMULA)
                {
                    datagrid.ItemsSource = m_lstTemplate;
                }
            }
            get { return _sheetModel; }
        }

        void Calc()
        {

            //2、组装计算公式为dictionary
            //3、发请求
            //4、刷新            
            var origin = new Dictionary<string, string>();
            foreach (var item in m_lstTemplate)
            {
                var lineNoJ = item.b;
                if (!string.IsNullOrEmpty(lineNoJ))
                {
                    origin.Add(lineNoJ + "y", item.c);
                    origin.Add(lineNoJ + "c", item.d);
                }
            }
            var filter = new Dictionary<string, object>();
            if (m_filter != null)
            {
                filter["year"] = m_filter["beginYear"];
                filter["period"] = m_filter["beginPeriod"];
            }
            
            var dest = executer.Calc(filter, origin);
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
            }
            datagrid.ItemsSource = lstResorce;
        }


        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_lstTemplate == null)
            {
                m_lstTemplate = DataFactory.Instance.GetTemplateExecuter().GetExcelTemplate("利润表");
                m_lstTemplate.Sort((a, b)=>
                {
                    var x = long.Parse(a.b);
                    var y = long.Parse(b.b);
                    if (x > y)
                        return 1;
                    else if (x == y)
                        return 0;
                    else
                        return -1;
                });
            }
            SheetModel = SheetModel.DATA;
        }

      
    }

    class ProfitSheetExportHandler : IExportHandler
    {
        public void Encode(ref DataTable data)
        {
            data.Columns["_a"].Caption = "项目";
            data.Columns["_b"].Caption = "行次";
            data.Columns["_c"].Caption = "本年累计金额";
            data.Columns["_d"].Caption = "本月金额";
            data.Columns.Remove("_e");
            data.Columns.Remove("_f");
            data.Columns.Remove("_g");
            data.Columns.Remove("_h");
        }
    }
}
