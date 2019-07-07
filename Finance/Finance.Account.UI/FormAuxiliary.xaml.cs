using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.Data.Model;
using Finance.Account.SDK;
using Finance.UI;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormUser.xaml 的交互逻辑
    /// </summary>
    public partial class FormAuxiliary : FinanceForm
    {
        public FormAuxiliary()
        {
            InitializeComponent();
            auxiliaryView.SelectedEvent += AuxiliaryView_Selected;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;                
                switch (txt)
                {
                    case "new":
                        AuxiliaryView_Selected(null);
                        break;
                    case "modify":
                        if (auxiliaryView.SelectedAuxiliaryObj == null)
                        {
                            FinanceMessageBox.Info("请选中一个项目");
                            return;
                        }
                        AuxiliaryView_Selected(auxiliaryView.SelectedAuxiliaryObj);
                        break;
                    case "delete":
                        var item = auxiliaryView.SelectedAuxiliaryObj;
                        if (item != null)
                        {
                            var ret = FinanceMessageBox.Quest(string.Format("确认要删除[{0}::{1}]吗？",item.no, item.name));
                            if (MessageBoxResult.Yes == ret)
                            {
                                DataFactory.Instance.GetAuxiliaryExecuter().Delete(item.id);
                                auxiliaryView.Refresh();
                            }
                        }
                        else
                        {
                            FinanceMessageBox.Info("请选中一个项目");
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

        void AuxiliaryView_Selected(AuxiliaryObj item)
        {
            var frm = new FormAuxiliaryPopup();
            if (item == null)
                item = new AuxiliaryObj { type = (long)auxiliaryView.SelectedType  };
            frm.ItemSource = JsonConvert.DeserializeObject<Auxiliary>(JsonConvert.SerializeObject(item));
            frm.AfterSaveEvent = new AfterSaveEventHandler(() =>
            {
                auxiliaryView.Refresh();
            });
            frm.ShowDialog();
        }
    }


}
