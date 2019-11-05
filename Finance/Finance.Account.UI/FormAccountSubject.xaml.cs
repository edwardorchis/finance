using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormVoucherList.xaml 的交互逻辑
    /// </summary>
    public partial class FormAccountSubject : FinanceForm
    {
        public FormAccountSubject()
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
                    case "new":
                        var frm = new FormAccountSubjectPopup();
                        frm.AfterSaveEvent = new AfterSaveEventHandler(()=> {
                            FinanceForm_Loaded(datagrid, null);
                        });
                        frm.ShowDialog();                        
                        break;
                    case "modify":
                        Popup();
                        break;
                    case "delete":
                        var item = datagrid.SelectedItem as AccountSubject;
                        if (item != null)
                        {
                            var ret = FinanceMessageBox.Quest(string.Format("确认要删除科目[{0}-{1}]吗？",item.no,item.name));
                            if (MessageBoxResult.Yes == ret)
                            {
                                DataFactory.Instance.GetAccountSubjectExecuter().Delete(item.id);
                                FinanceForm_Loaded(datagrid, null);
                            }
                        }
                        else
                        {
                            FinanceMessageBox.Info("请选中一个科目");
                        }
                        break; 
                    case "disable":
                        var item1 = datagrid.SelectedItem as AccountSubject;
                        if (item1 != null)
                        {
                            var ret = FinanceMessageBox.Quest(string.Format("确认要禁用科目[{0}-{1}]吗？", item1.no, item1.name));
                            if (MessageBoxResult.Yes == ret)
                            {
                                DataFactory.Instance.GetAccountSubjectExecuter().SetStatus(item1.id,1);
                                FinanceForm_Loaded(datagrid, null);
                            }
                        }
                        else
                        {
                            FinanceMessageBox.Info("请选中一个科目");
                        }
                        break;
                    case "enable":
                        var item2 = datagrid.SelectedItem as AccountSubject;
                        if (item2 != null)
                        {
                            var ret = FinanceMessageBox.Quest(string.Format("确认要启用科目[{0}-{1}]吗？", item2.no, item2.name));
                            if (MessageBoxResult.Yes == ret)
                            {
                                DataFactory.Instance.GetAccountSubjectExecuter().SetStatus(item2.id,0);
                                FinanceForm_Loaded(datagrid, null);
                            }
                        }
                        else
                        {
                            FinanceMessageBox.Info("请选中一个科目");
                        }
                        break;
                    case "refresh":
                        FinanceForm_Loaded(datagrid,null);
                        break;
                   
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        string mSpaceString = "                         ";
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.AccountGroupList, DataFactory.Instance.GetAuxiliaryExecuter().List(AuxiliaryType.AccountGroup));
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AccountSubjectList);
            var lst = DataFactory.Instance.GetAccountSubjectExecuter().List();            
            var json = JsonConvert.SerializeObject(lst);
            var src = JsonConvert.DeserializeObject<List<AccountSubject>>(json);
            src.ForEach(aso=> { aso.no = mSpaceString.Substring(0, (aso.level -1) * 4) + aso.no; });
            datagrid.ItemsSource = src;            
        }

        private void datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as AccountSubject;
            if (drv == null)
                return;
            if (drv.isDeleted)
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            else
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void datagrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Popup();
        }

        void Popup()
        {
            var item = datagrid.SelectedItem as AccountSubject;
            if (item != null)
            {
                var frm = new FormAccountSubjectPopup();
                frm.ItemSource = item;
                frm.AfterSaveEvent = new AfterSaveEventHandler(() => {
                    FinanceForm_Loaded(datagrid, null);
                });
                frm.ShowDialog();
            }
            else {
                FinanceMessageBox.Info("请选中一个科目");
            }
        }
    }


}
