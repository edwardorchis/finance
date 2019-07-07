using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.Data.Model;
using Finance.Account.SDK;
using Finance.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormUser.xaml 的交互逻辑
    /// </summary>
    public partial class FormCarriedForwardTemplate : FinanceForm
    {
        public FormCarriedForwardTemplate()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;
                var lst = datagrid.ItemsSource as List<CarriedForwardTemplate>;
                //switch (txt)
                //{
                    //case "save":
                    //    if (lst.Count > 0)
                    //    {
                    //        DataFactory.Instance.GetTemplateExecuter().SaveCarriedForwardTemplate(lst);
                    //    }
                    //    FinanceMessageBox.Info("保存成功");
                    //    ListView_SelectionChanged(null,null);
                    //    break;
                    //case "append":
                    //    var item = listView.SelectedItem as Auxiliary;

                    //    lst.Add(new CarriedForwardTemplate { id = item.id });

                    //    datagrid.ItemsSource = null;
                    //    datagrid.ItemsSource = lst;
                    //    break;
                    //case "delete":
                    //    if (datagrid.SelectedItem == null)
                    //        return;
                    //    var tmp = datagrid.SelectedItem as CarriedForwardTemplate;
                    //    if (tmp == null)
                    //        return;
                    //    lst.Remove(tmp);
                    //    datagrid.ItemsSource = null;
                    //    datagrid.ItemsSource = lst;
                    //    break;
                //}               
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
            listView.ItemsSource = lst;
            if (lst.Count > 0)
                listView.SelectedIndex = 0;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listView.SelectedItem == null)
                    return;

                var aux = listView.SelectedItem as Auxiliary;
                var lst = DataFactory.Instance.GetTemplateExecuter().ListCarriedForwardTemplate(aux.id);
                lst.Sort((a, b) => { return a.index > b.index ? 1 : 0; });
                var displayList = new List<CarriedForwardTemplateDisplayItem>();
                lst.ForEach(a=> {
                    var srcObj = DataFactory.Instance.GetAccountSubjectExecuter().Find(a.src);
                    var dstObj = DataFactory.Instance.GetAccountSubjectExecuter().Find(a.dst);

                    var item = new CarriedForwardTemplateDisplayItem();
                    item.srcNo = srcObj.no;
                    item.srcName = srcObj.fullName;
                    item.dstNo = dstObj.no;
                    item.dstName = dstObj.fullName;
                    displayList.Add(item);
                });
                datagrid.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }
        }

    }

    public class CarriedForwardTemplateDisplayItem
    {
        public string srcNo { set; get; }
        public string srcName { set; get; }
        public string dstNo { set; get; }
        public string dstName { set; get; }
    }
}
