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

namespace FinanceClient
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var lst = new List<ContentInfo>();
            lst.Add(new ContentInfo { no = "01", name = "零壹" });
            lst.Add(new ContentInfo { no = "02", name = "零贰" });
            lst.Add(new ContentInfo { no = "03", name = "零叁" });
            lst.Add(new ContentInfo { no = "04", name = "零肆" });
            cmb1.ItemsSource = lst;
        }

        public class ContentInfo
        {
            public string no { set; get; }

            public string name { set; get; }
        }
    }
}
