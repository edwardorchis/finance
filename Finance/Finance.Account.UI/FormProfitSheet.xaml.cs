using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.UI;
using Finance.Utils;
using Newtonsoft.Json;
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
                        frmFilter.FilterPopupEvent += (args) =>
                        {
                            m_filter = args.Filter;
                            //Refresh();
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

            var dest = executer.Calc(origin);
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
            if(m_lstTemplate==null)
                m_lstTemplate = DataFactory.Instance.GetTemplateExecuter().GetExcelTemplate("利润表");
            SheetModel = SheetModel.DATA;
        }

      
    }


}
