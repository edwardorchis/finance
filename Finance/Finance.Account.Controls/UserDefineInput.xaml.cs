using Finance.Account.Controls.Commons;
using Finance.Utils;
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

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(UserDefineInput), new PropertyMetadata(""));

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
            if (IsEnabled && TagLabel.StartsWith("comb|"))
            {
                DisplayCombBox();
                return;
            }
            xCombBox.Visibility = Visibility.Hidden;
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

        void DisplayCombBox()
        {
            var dict = JsonConverter.JsonDeserialize<Dictionary<string, string>>(TagLabel.Substring(5));
            xCombBox.ItemsSource = dict;
            xCombBox.SelectedValue = Convert.ToString(mValue);
            xCombBox.GotFocus += new RoutedEventHandler((sender, e) => {
                (sender as ComboBox).IsDropDownOpen = true;
            });
            xCombBox.KeyDown += new KeyEventHandler((sender, e) => {
                if (e.Key == Key.Enter)
                {
                    KeyValuePair<string, string> obj = dict.FirstOrDefault(kv=>kv.Value.StartsWith(xCombBox.Text));
                    if (obj.Key != null)
                    {
                        xCombBox.SelectedValue = obj.Key;
                        xCombBox.Text = obj.Value;
                    }
                    Commons.Keyboard.Press(Key.Tab);
                }
            });
            xCombBox.LostFocus += new RoutedEventHandler((sender, e) =>
            {
                mValue = xCombBox.SelectedValue;
                Console.WriteLine(string.Format("DisplayCombBox: {0}", mValue == null ? "null" : mValue.ToString()));
            });
            xString.Visibility = Visibility.Hidden;            
        }

        private void XString_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Control tb = sender as Control;
            if (tb != null)
            {
                tb.Focus();
                e.Handled = true;
            }
        }

        bool gFocused = false;
        private void ThisInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (xString.Visibility != Visibility.Hidden)
                SetFocus(xString);
            else if (!gFocused)
            {
                SetFocus(xCombBox);
                gFocused = true;
            }
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
