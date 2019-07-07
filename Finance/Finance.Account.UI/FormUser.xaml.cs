using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.Data.Model;
using Finance.Account.SDK;
using Finance.UI;
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
    public partial class FormUser : FinanceForm
    {
        public FormUser()
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
                        var frm = new FormUserPopup();
                        frm.AfterSaveEvent = new AfterSaveEventHandler(()=> {
                            FinanceForm_Loaded(datagrid, null);
                        });
                        frm.ShowDialog();                        
                        break;
                    case "modify":
                        Popup();
                        break;
                    case "disable":
                        var item = datagrid.SelectedItem as User;
                        if (item != null)
                        {
                            var ret = FinanceMessageBox.Quest(string.Format("确认要禁用用户[{0}]吗？",item.Name));
                            if (MessageBoxResult.Yes == ret)
                            {
                                DataFactory.Instance.GetUserExecuter().Disable(item.Id);
                                FinanceForm_Loaded(datagrid, null);
                            }
                        }
                        else
                        {
                            FinanceMessageBox.Info("请选中一个用户");
                        }
                        break;
                    case "enable":
                        var item1 = datagrid.SelectedItem as User;
                        if (item1 != null)
                        {
                            DataFactory.Instance.GetUserExecuter().Enable(item1.Id);
                            FinanceForm_Loaded(datagrid, null);
                        }
                        else
                        {
                            FinanceMessageBox.Info("请选中一个用户");
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

 
      
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.UserList);
            datagrid.ItemsSource = DataFactory.Instance.GetUserExecuter().List();            
        }

        private void datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as User;
            if (drv == null)
                return;
            if (drv.IsDeleted)
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
            var item = datagrid.SelectedItem as User;
            if (item != null)
            {
                var frm = new FormUserPopup();
                frm.UserId = item.Id;
                frm.UserName = item.Name;                
                frm.AfterSaveEvent = new AfterSaveEventHandler(() => {
                    FinanceForm_Loaded(datagrid, null);
                });
                frm.ShowDialog();
                FinanceForm_Loaded(datagrid, null);
            }
            else {
                FinanceMessageBox.Info("请选中一个用户");
            }
        }
    }


}
