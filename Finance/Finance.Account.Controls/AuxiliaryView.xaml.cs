using Finance.Account.Controls.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Finance.Account.Controls
{
    /// <summary>
    /// AuxiliaryView.xaml 的交互逻辑
    /// </summary>
    public partial class AuxiliaryView : UserControl
    {
        public delegate void SelectedEventHandler(AuxiliaryObj auxiliaryObj);
        public SelectedEventHandler SelectedEvent;
        public AuxiliaryView()
        {
            InitializeComponent();
            datagrid.MouseDoubleClick += Datagrid_MouseDoubleClick;
        }
        public AuxiliaryType Type { set; private get; } = AuxiliaryType.Invalid;

        public void Refresh()
        {
            ListView_SelectionChanged(listView, null);

        }

        public AuxiliaryType SelectedType { get; private set; }
        public AuxiliaryGroup AuxGrp { set; get; }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItem == null)
                return;
            var selected = (KeyValuePair<int, string>)listView.SelectedItem;
            SelectedType = (AuxiliaryType)selected.Key;
            List<AuxiliaryObj> lst = AuxiliaryList.Get((AuxiliaryType)selected.Key);
            datagrid.ItemsSource = lst;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            listView.Items.Clear();
            var dict = AuxiliaryList.GetGroupTypes(AuxGrp);
            foreach (var kv in dict)
            {
                if (Type > AuxiliaryType.Invalid && Type < AuxiliaryType.Max)
                {
                    if (kv.Key != (int)Type)
                        continue;
                }
                listView.Items.Add(kv);
            }
            if (listView.Items.Count > 0)
                listView.SelectedIndex = 0;

        }

        public AuxiliaryObj SelectedAuxiliaryObj {
            get {
                return datagrid.SelectedItem as AuxiliaryObj;                
            }
        }

        private void Datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = datagrid.SelectedItem as AuxiliaryObj;
            if (item != null)
            {
                SelectedEvent?.Invoke(item);
            }
            else
            {
                FinanceMessageBox.Info("请选中一个项目");
            }
        }

    }
}
