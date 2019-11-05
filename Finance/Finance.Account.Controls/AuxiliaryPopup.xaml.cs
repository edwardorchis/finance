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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AuxiliaryPopup : Window
    {
        public delegate void SelectedEventHandler(AuxiliaryObj auxiliaryObj);     
        public SelectedEventHandler SelectedEvent;
        AuxiliaryType mType = AuxiliaryType.Invalid;
        public AuxiliaryPopup(AuxiliaryType type)
        {
            InitializeComponent();
            mType = type;
        }
        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        public AuxiliaryGroup AuxGrp { set; get; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listView.Items.Clear();
            var dict = AuxiliaryList.GetGroupTypes(AuxGrp);
            foreach (var kv in dict)
            {
                if (mType > AuxiliaryType.Invalid && mType < AuxiliaryType.Max)
                {
                    if (kv.Key != (int)mType)
                        continue;
                }
                listView.Items.Add(kv);                            
            }
            if(listView.Items.Count > 0)
                listView.SelectedIndex = 0;

            datagrid.MouseDoubleClick += Datagrid_MouseDoubleClick;
        }

        private void Datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = datagrid.SelectedItem as AuxiliaryObj;
            if (item != null)
            {
                SelectedEvent?.Invoke(item);
                this.Close();
            }
            else
            {
                FinanceMessageBox.Info("请选中一个项目");
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (KeyValuePair<int, string>)listView.SelectedItem;
            List<AuxiliaryObj> lst = AuxiliaryList.Get((AuxiliaryType)selected.Key);
            datagrid.ItemsSource = lst;
        }
    }
}
