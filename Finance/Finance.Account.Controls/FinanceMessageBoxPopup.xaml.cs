using Finance.Account.Controls.Commons;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static Finance.Account.Controls.FinanceMessageBox;

namespace Finance.Account.Controls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    internal partial class FinanceMessageBoxPopup : Window
    {
        public delegate void ButtonClickEventHandler(int num);
        public ButtonClickEventHandler ButtonClickEvent;

        public delegate void ProgressEventHandler(ProgressEventArgs e);
        public ProgressEventHandler ProgressEvent;

       
        public FinanceMessageBoxPopup()
        {
            InitializeComponent();
            this.Loaded += FinanceMessageBoxPopup_Loaded;
        }

        string btn1Text, btn2Text, btn3Text;
        DispatcherTimer _timer = new DispatcherTimer();

        public FinanceMessageBoxPopup(string btnTxt1,string btnTxt2, string btnTxt3)
        {
            InitializeComponent();
            btn1Text = btnTxt1;
            btn2Text = btnTxt2;
            btn3Text = btnTxt3;
            this.Loaded += FinanceMessageBoxPopup_Loaded;
        }
        
        private void FinanceMessageBoxPopup_Loaded(object sender, RoutedEventArgs e)
        {
            txtBox.Text = Message;

            btn1.Visibility = string.IsNullOrEmpty(btn1Text) ? Visibility.Hidden : Visibility.Visible;            
            btn2.Visibility = string.IsNullOrEmpty(btn2Text) ? Visibility.Hidden : Visibility.Visible;            
            btn3.Visibility = string.IsNullOrEmpty(btn3Text) ? Visibility.Hidden : Visibility.Visible;
            btn1.Content = btn1Text;
            btn2.Content = btn2Text;
            btn3.Content = btn3Text;

            if (!Wait && btn1.Visibility == Visibility.Hidden && btn2.Visibility == Visibility.Hidden && btn3.Visibility == Visibility.Hidden)
            {
                btn3.Visibility = Visibility.Visible;
                btn3.Content = "确定";
                btn3.IsDefault = true;
            }

            if (Wait)
            {
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                EventHandler event1 = new EventHandler(timer_Tick);
                _timer.Tick += event1;
                _timer.Tag = txtBox;
                _timer.Start();
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ProgressEventArgs args = new ProgressEventArgs();
            args.Message = Message;
            args.Close = false;
            ProgressEvent?.Invoke(args);

            txtBox.Text = args.Message;
            if (args.Close)
                Close();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var str = (sender as Button).Name;
            var numStr = str.Substring(str.Length - 1);
            ButtonClickEvent?.Invoke(int.Parse(numStr));
            Close();
        }

        public string Message { set; get; }

        public bool Wait { set; get; }
    }
}
