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
    public partial class FormCarriedForward : FinanceForm
    { 
        public FormCarriedForward()
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
                        var proc = procName;
                        Dictionary<string, object> paramMap = new Dictionary<string, object>();
                        paramMap.Add("word", cmbWord.SelectedValue);
                        paramMap.Add("explanation", txtExplanation.Text);

                        Task task = Task.Run(() =>
                        {
                            try
                            {                               
                                taskId = DataFactory.Instance.GetInterfaceExecuter().ExecTask(ExecTaskType.CarriedForward, proc, paramMap);
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
                            
                        txtResult.AppendText(message + "\r\n");
                        break;                   
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }
        bool isShow = false;
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (isShow)
                return;
            isShow = true;
            var lst = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.CarriedForward);
            if (lst == null)
                return;
         
            var lstCmb = new Dictionary<string, string>();
            lst.ForEach(tmp => {
                if (!lstCmb.ContainsKey(tmp.no))
                    lstCmb.Add(tmp.no, tmp.name);
            });
            cmbProcName.ItemsSource = lstCmb;
            if (lstCmb.Count > 0)
                cmbProcName.SelectedIndex = 0;

            var lstWord = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.ProofOfWords);
        
            cmbWord.ItemsSource = lstWord;
            cmbWord.SelectedValue = "转";
        }

        string procName
        {
            get { return (string)GetValue(procNameProperty); }
            set { SetValue(procNameProperty, value); }
        }

        static readonly DependencyProperty procNameProperty =
            DependencyProperty.Register("procName", typeof(string), typeof(FormCarriedForward), new PropertyMetadata(""));

        string word
        {
            get { return (string)GetValue(wordProperty); }
            set { SetValue(wordProperty, value); }
        }

        static readonly DependencyProperty wordProperty =
            DependencyProperty.Register("word", typeof(string), typeof(FormCarriedForward), new PropertyMetadata("转"));

        private void CmbProcName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = (KeyValuePair<string, string>)e.AddedItems[0];
                txtExplanation.Text = item.Value;
            }
         
        }
    }


}
