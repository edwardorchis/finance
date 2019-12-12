using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.Data.Model;
using Finance.Account.SDK;
using Finance.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormUser.xaml 的交互逻辑
    /// </summary>
    public partial class FormAccesRight : FinanceForm
    {
        public FormAccesRight()
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
                    case "save":
                        var lst = datagrid.ItemsSource as List<AccessRightListItem>;
                        var saveData = new List<AccessRight>();
                        lst.ForEach(item=> {                            
                            if (listView.SelectedItem != null)
                            {
                                var user = listView.SelectedItem as User;
                                var access = new AccessRight();
                                access.group = item.first;
                                access.name = item.second;
                                access.mask = item.isAllow ? 1 : 0;
                                access.id = user.Id;
                                saveData.Add(access);
                            }
                        });
                        if (saveData.Count > 0)
                        {
                            DataFactory.Instance.GetSystemProfileExecuter().SaveAccessRight(saveData);
                        }
                        FinanceMessageBox.Info("保存成功");
                        break;
                    case "allow":
                        CheckAll(true);
                        break;
                    case "unallow":
                        CheckAll(false);
                        break;                  
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void CheckAll(bool bChecked)
        {           
            var lst = datagrid.ItemsSource as List<AccessRightListItem>;
            lst.ForEach(item => item.isAllow = bChecked);
            datagrid.ItemsSource = lst;
            for (int i = 0; i < datagrid.Items.Count; i++)
            {
                DataGridRow neddrow = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(i);
                CheckBox cb = (CheckBox)datagrid.Columns[4].GetCellContent(neddrow);
                cb.IsChecked = bChecked;
            }
        }

        List<MenuTableMap> mMenuList = new List<MenuTableMap>();
        List<User> mUserList = new List<User>();
        bool isShow = false;     
        private void FinanceForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (isShow)
                return;
            isShow = true;
            var userList = DataFactory.Instance.GetUserExecuter().List();
            mUserList.Clear();
            userList.ForEach(u=> {
                if (u.Name != "admin")
                    mUserList.Add(u);
            });
            listView.ItemsSource = mUserList;
            if (mUserList.Count > 0)
            {
                mMenuList = DataFactory.Instance.GetSystemProfileExecuter().GetAllMenuTables();
                listView.SelectedIndex = 0;               
            }
                
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listView.SelectedItem == null)
                    return;

                var user = listView.SelectedItem as User;

                var lstAccessRight = DataFactory.Instance.GetSystemProfileExecuter().GetAccessRight(user.Id);

                var lst = new List<AccessRightListItem>();
                mMenuList.ForEach(menu =>
                {
                    var item = new AccessRightListItem { first = menu.group, firstName = UiUtils.FirstMenuDisplayNameMap[menu.group], second = menu.name, secondName = menu.header };
                    var access = lstAccessRight.FirstOrDefault(a => a.group == menu.group && a.name == menu.name);
                    if (access != null && access.mask > 0)
                        item.isAllow = true;
                    lst.Add(item);
                });

                datagrid.ItemsSource = lst;
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }
        }
    }

    


    internal class AccessRightListItem
    {
        public string first { set; get; }
        public string firstName { set; get; }
        public string second { set; get; }
        public string secondName { set; get; }
        public bool isAllow { set; get; }
    }

}
