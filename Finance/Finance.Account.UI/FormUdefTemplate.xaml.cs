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
    public partial class FormUdefTemplate : FinanceForm
    {
        public FormUdefTemplate()
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
                        var frm = new FormUdefTemplatePopup();
                        frm.AfterSaveEvent = new AfterSaveEventHandler(()=> {
                            FinanceForm_Loaded(datagrid, null);
                        });
                        frm.ShowDialog();                        
                        break;
                    case "modify":
                        Popup();
                        break;
                    case "delete":
                        foreach (var item in datagrid.SelectedItems)
                        {
                            var udefTemp = item as UdefTemplateItem;
                            if(udefTemp != null)
                                DataFactory.Instance.GetTemplateExecuter().DeleteUdefTemplate(udefTemp);
                        }
                        FinanceForm_Loaded(null, null);
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
            var lst = DataFactory.Instance.GetTemplateExecuter().GetUdefTemplate("");
            datagrid.ItemsSource = lst;
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
            var item = datagrid.SelectedItem as UdefTemplateItem;
            if (item != null)
            {
                var frm = new FormUdefTemplatePopup();
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
