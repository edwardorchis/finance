
using Arthas.Controls.Metro;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FinanceAcountManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = ConfigHelper.XmlReadAppSetting("FinanceClient.exe.config", "copyright");
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            
        }
    }
}
