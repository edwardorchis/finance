using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.Data.Model;
using Finance.Account.SDK;
using Finance.UI;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace Finance.Account.UI
{
    public partial class FormInterface : FinanceForm
    {
        List<UdefTemplateItem> mUdefTemplate = new List<UdefTemplateItem>();
        List<UserDefineInputItem> mUserDefineInputItems = new List<UserDefineInputItem>();       
        public FormInterface()
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
                    case "exec":                       
                        bool bEnd = false;
                        bool bTmp = false;
                        string message = "";
                        string taskId = "";
                        var filter = ReadFilter();
                        var proc = procName;
                        Task task = Task.Run(() =>
                        {
                            try
                            {
                                taskId = DataFactory.Instance.GetInterfaceExecuter().ExecTask(ExecTaskType.CreateVoucher, proc, filter);
                            }
                            catch (Exception ex)
                            {
                                message = ex.Message;
                            }
                            bEnd = true;
                        });
                        FinanceMessageBox.Progress("开始执行...", (args) =>
                        {
                            bTmp = !bTmp;
                            args.Message = "正在执行 " + (bTmp ? "..." : "");
                            args.Close = bEnd;
                        });
                        if (string.IsNullOrEmpty(message))
                        {
                            var taskResult = DataFactory.Instance.GetInterfaceExecuter().GetTaskResult(taskId);
                            if (taskResult != null)
                                message = taskResult.result;
                        }
                            
                        txtResult.AppendText(message);
                        break;                   
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        Dictionary<string, object> ReadFilter()
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            mUserDefineInputItems = userDefinePanel.DataSource;
            mUserDefineInputItems.ForEach(udef => {
                if (map.ContainsKey(udef.Name))
                    map[udef.Name] = udef.DataValue;
                else
                    map.Add(udef.Name, udef.DataValue);
            });
            return map;
        }


        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            var lst = DataFactory.Instance.GetTemplateExecuter().GetUdefTemplate("_InterfaceExec");
            if (lst == null)
                return;
            mUdefTemplate = lst;
            var lstCmb = new Dictionary<string, string>();
            lst.ForEach(tmp => {
                if (!lstCmb.ContainsKey(tmp.reserved))
                    lstCmb.Add(tmp.reserved, string.IsNullOrEmpty(tmp.tagLabel) ? tmp.reserved :tmp.tagLabel);
            });
            cmbProcName.ItemsSource = lstCmb;
            if (lstCmb.Count > 0)
                cmbProcName.SelectedIndex = 0;
        }

        string procName
        {
            get { return (string)GetValue(procNameProperty); }
            set { SetValue(procNameProperty, value); }
        }

        static readonly DependencyProperty procNameProperty =
            DependencyProperty.Register("procName", typeof(string), typeof(FormInterface), new PropertyMetadata(""));

      
        private void LoadParms()
        {          
            var lst = mUdefTemplate.FindAll(tmp => tmp.reserved == procName);
            if (lst == null)
                return;
            mUserDefineInputItems.Clear();
            foreach (var item in lst)
            {
                object val = item.defaultVal;
                //if (filter != null && filter.ContainsKey(item.name))
                //{
                //    val = filter[item.name];
                //}

                mUserDefineInputItems.Add(new UserDefineInputItem
                {
                    Label = item.label,
                    DataType = CommonUtils.ConvertDataTypeFromStr(item.dataType),
                    Name = item.name,
                    DataValue = val,
                    TabIndex = item.tabIndex,
                    TagLabel = string.IsNullOrEmpty(item.tagLabel) ? "" : item.tagLabel,
                    Width = item.width
                });
            }

            mUserDefineInputItems = mUserDefineInputItems.OrderBy(item => item.TabIndex).ToList();
            userDefinePanel.DataSource = mUserDefineInputItems;
        }

        private void CmbProcName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadParms();
        }
    }


}
