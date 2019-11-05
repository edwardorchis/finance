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
using static Finance.Account.Controls.Commons.Consts;

namespace Finance.Account.Controls
{
    /// <summary>
    /// UserDefinePanel.xaml 的交互逻辑
    /// </summary>
    public partial class UserDefinePanel : UserControl
    {
        public UserDefinePanel()
        {
            InitializeComponent();           
        }

        public DataChangedEventHandler DataChangedEvent;
        
        List<UserDefineInputItem> mDataSource = new List<UserDefineInputItem>();
        public List<UserDefineInputItem> DataSource {
            set {
                var val = value;
                CheckDataSource(val);
                mDataSource = val;
                Display();
            }
            get {
                RebuildDataSource();
                return mDataSource;
            }
        }

        void CheckDataSource(List<UserDefineInputItem> lst)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            lst.ForEach(item=> {
                if (dict.ContainsKey(item.Name))
                    throw new Exception("UserDefineInputItem's name must be unique.");
                dict.Add(item.Name,item.Name);
            });
        }

        bool mCanceled = false;
        List<string> mChildrenList = new List<string>();
        void Display()
        {  
            mChildrenList.ForEach(n => xPanel.UnregisterName(n));            
            xPanel.Children.Clear();
            mChildrenList.Clear();
            foreach(var item in mDataSource)
            {
                UserDefineInput input = new UserDefineInput();
                input.IsEnabled = this.IsEnabled;
                input.Width = item.Width == 0 ? 220 : item.Width;
                input.TagLabel = item.TagLabel;
                input.Label = item.Label;
                input.DataType = item.DataType;
                input.DataValue = item.DataValue;
                input.Margin = new Thickness(0,5,0,0);
                input.Name = this.Name + "_" + item.Name;
                input.Unit = item.Unit;
                input.DataChangedEvent += new DataChangedEventHandler((sender,e)=> {
                    DataChangedEvent?.Invoke(sender, e);
                    if (e.Cancel)                   
                    {
                        mCanceled = true;
                    }                   
                });                
                xPanel.RegisterName(input.Name, input);
                mChildrenList.Add(input.Name);
                xPanel.Children.Add(input);
            }       
        }

        public UserDefineInput FindInputByName(string name)
        {
            UserDefineInput input = xPanel.FindName(this.Name + "_" + name) as UserDefineInput;
            return input;
        }

        void RebuildDataSource()
        {
            if (mCanceled)
                return;
            mChildrenList.ForEach(n=> {
                UserDefineInput input = xPanel.FindName(n) as UserDefineInput;
                if (input != null)
                {
                    mDataSource.FirstOrDefault(m => this.Name + "_" + m.Name == n).DataValue = input.DataValue;
                }
            });
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mDataSource.Count == 0)
                return;
            var firstEleName = this.Name + "_" + mDataSource.First().Name;
            var ele = xPanel.FindName(firstEleName) as FrameworkElement;
            if (ele == null)
                return;
            SetFocus(ele);
        }

        void SetFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
            new Action(() =>
            {
                var suc = element.Focus();
                System.Windows.Input.Keyboard.Focus(element);
            }));
        }
    }
}
