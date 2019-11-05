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
    public partial class FormMenuEdit : FinanceForm
    {
        public FormMenuEdit()
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
                        var frm = new FormMenuEditPopup();
                        frm.AfterSaveEvent = new AfterSaveEventHandler(()=> {
                            FinanceForm_Loaded(datagrid, null);
                        });
                        frm.ShowDialog();                        
                        break;
                    case "modify":
                        Popup();
                        break;
                    case "delete":
                        var item = datagrid.SelectedItem as MenuTableMap;
                        if(item != null)
                            DataFactory.Instance.GetSystemProfileExecuter().DeleteMenuItem(item);
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
            var lst = DataFactory.Instance.GetSystemProfileExecuter().GetMenuTables();
            datagrid.ItemsSource = lst;
        }
    
        private void datagrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Popup();
        }

        void Popup()
        {
            var item = datagrid.SelectedItem as MenuTableMap;
            if (item != null)
            {
                var frm = new FormMenuEditPopup();
                frm.ItemSource = item;
                frm.AfterSaveEvent = new AfterSaveEventHandler(() => {
                    FinanceForm_Loaded(datagrid, null);
                });
                frm.ShowDialog();
                FinanceForm_Loaded(datagrid, null);
            }
            else {
                FinanceMessageBox.Info("请选中一个项目");
            }
        }
    }


}
