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
    /// UserDefineInput.xaml 的交互逻辑
    /// </summary>
    public partial class UserDefineInput : UserControl
    {        
        public DataChangedEventHandler DataChangedEvent;
        public UserDefineInput()
        {
            InitializeComponent();
        }

        public string TagLabel { set; get; }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(UserDefineInput), new PropertyMetadata(""));

        public Type DataType { set; get; } = typeof(string);
        
        object mValue;
        public object DataValue {
            set {                
                if (!CheckValueType(value))
                    mValue = DataType.IsValueType ? Activator.CreateInstance(DataType) : null;
                else
                    mValue = value;
                Display();
            } get {
                return mValue;
            }
        }

        bool CheckValueType(object val)
        {
            try
            {              
                mValue = Convert.ChangeType(val, DataType);
                return true;
            }
            catch
            {
              
            }
            return false;
        }
        /// <summary>
        /// 显示
        /// </summary>
        void Display()
        {
            if (mValue == null)
                xString.Text = "";
            else
                xString.Text = Convert.ToString(mValue);

            if (!IsEnabled)
            {
                xString.BorderThickness = new Thickness(0,0,0,1);
                return;
            }

            if (DataType == typeof(long) || DataType == typeof(decimal) || DataType == typeof(byte)
                || DataType == typeof(sbyte) || DataType == typeof(short) || DataType == typeof(int)
                || DataType == typeof(ushort) || DataType == typeof(uint) || DataType == typeof(ulong)
                || DataType == typeof(float) || DataType == typeof(double))
                xString.HorizontalContentAlignment = HorizontalAlignment.Right;
            xString.PreviewMouseDown += XString_PreviewMouseDown;
            xString.GotFocus += new RoutedEventHandler((sender, e) =>
            {
                xString.SelectAll();
                xString.PreviewMouseDown -= XString_PreviewMouseDown;
                e.Handled = true;
            });
            xString.KeyDown += new KeyEventHandler((sender, e)=> {
                if (e.Key == Key.Enter)
                {
                    Commons.Keyboard.Press(Key.Tab);
                }
            });
            xString.LostFocus += new RoutedEventHandler((sender, e) => {
                xString.PreviewMouseDown += XString_PreviewMouseDown;           
                object defaultValue = DataType.IsValueType ? Activator.CreateInstance(DataType) : null;
                bool err = false;
                try
                {
                    object oldValue = mValue;
                    mValue = Convert.ChangeType(xString.Text, DataType);
                    if (mValue == defaultValue)
                    {
                        throw new Exception();
                    }
                    if (oldValue == mValue)
                        return;
                    DataChangedArgs args = new DataChangedArgs
                    {
                        Name = this.Name,
                        OldValue = oldValue,
                        NewValue = mValue,                        
                        Cancel = false,
                        DataType = DataType
                    };
                    DataChangedEvent?.Invoke(this, args);
                    if (args.Cancel)
                    {
                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                         new Action(() =>
                         {
                             if (oldValue == null)
                                 xString.Text = "";
                             else
                                 xString.Text = Convert.ToString(oldValue);
                             if (err)
                                 xString.SelectAll();
                         }));
                        e.Handled = true;
                    }
                }
                catch
                {
                    mValue = defaultValue;
                    e.Handled = true;
                    err = true;
                }
                finally
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                      new Action(() =>
                      {
                          if (mValue == null)
                              xString.Text = "";
                          else
                              xString.Text = Convert.ToString(mValue);
                          if(err)
                            xString.SelectAll();
                      }));
                }
            });
        }

        private void XString_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.Focus();
                e.Handled = true;
            }
        }

        private void ThisInput_GotFocus(object sender, RoutedEventArgs e)
        {
            SetFocus(xString);
            e.Handled = true;
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
