using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Finance.Account.UI.Model.Constant;

namespace Finance.Account.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    internal partial class FormUdefReportFilterPopup : Window
    {
        public FilterPopupEventHandler FilterPopupEvent;
        public Dictionary<string, object> Filter { set; get; }
        List<UserDefineInputItem> mUserDefineInputItems = new List<UserDefineInputItem>();
        FormUdefReport mOwner = null;
        public FormUdefReportFilterPopup(FormUdefReport owner)
        {
            InitializeComponent();
            mOwner = owner;
        }     

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (FilterPopupEvent != null)
            {
                FilterPopupEventArgs args = new FilterPopupEventArgs { Filter = ReadFilter()};
                try
                {
                    FilterPopupEvent(args);
                }
                catch (Exception ex)
                {
                    FinanceMessageBox.Error(ex.Message);
                }                
            }
            Close();
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

        void LoadFilter(Dictionary<string, object> filter)
        {
            var lst = DataFactory.Instance.GetTemplateExecuter().GetUdefTemplate(mOwner.Name);
            if (lst == null)
                return;
            foreach (var item in lst)
            {
                object val = item.defaultVal;
                if (filter != null && filter.ContainsKey(item.name))
                {
                    val = filter[item.name];
                }               
                    
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             LoadFilter(Filter);
        }
    }
}
