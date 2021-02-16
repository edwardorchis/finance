using Finance.Account.Controls.Commons;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.Account.Controls.AccountSubjectPopup;
using static Finance.Account.Controls.Commons.Consts;
using Keyboard = Finance.Account.Controls.Commons.Keyboard;

namespace Finance.Account.Controls
{
    /// <summary>
    /// AccountSubjectBox.xaml 的交互逻辑
    /// </summary>
    public partial class AccountSubjectBox : TextBox
    {
        public DataChangedEventHandler DataChangedEvent;
        public DisplayHookEventHandler DisplayHookEvent;
        public AccountSubjectBox()
        {
            InitializeComponent();
            base.TextWrapping = TextWrapping.WrapWithOverflow;
            base.KeyDown += OnKeyDown;
            base.LostFocus += OnLostFocus;
            base.GotFocus += OnGotFocus;
            base.MouseDoubleClick += AccountSubjectBox_MouseDoubleClick;
        }

        private void AccountSubjectBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (base.IsReadOnly)
                return;
            AccountSubjectPopup popupForm = new AccountSubjectPopup();
            popupForm.SelectedEvent += OnSelected;
            popupForm.ShowDialog();
        }

        public AccountSubjectObj Value { get; private set; } = new AccountSubjectObj();


        public long Id
        {
            set
            {
                Value = AccountSubjectList.Find(value);
                if (DisplayHookEvent != null)
                {
                    var args = new DisplayHookArgs();
                    args.Value = Value;
                    DisplayHookEvent.Invoke(this, args);
                    SetValue(TextProperty, args.Text);
                }
                else
                {
                    SetValue(TextProperty, Value.FullName);
                }
                SetValue(IdProperty, value);
            }
            get {
                SetValue(IdProperty, Value.id);
                return (long)GetValue(IdProperty);
                //return Value.id;
            }
        }
        static readonly DependencyProperty IdProperty =
         DependencyProperty.Register("Id", typeof(long), typeof(AccountSubjectBox));

        public string No
        {
            get
            {
                return Value.no;
            }            
        }

        public string FullName
        {
            get
            {
                return Value.FullName;
            }
        }
        private long oldId;
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (base.IsReadOnly)
                return;

            oldId = Id;
            base.Text = No;
            base.Background = Consts.HIGHLIGHT_BRUSH;
            //FocusEvent?.Invoke(this);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (base.IsReadOnly)
                return;            
            base.Background = Consts.WHITE_BRUSH;
            var txt = GetValue(TextProperty).ToString();
            var obj = AccountSubjectList.FindByNo(txt.Trim());
            Value = obj;
            if (Id != oldId)
            {
                DataChangedEvent?.Invoke(this, 
                    new DataChangedArgs {
                        OldValue = oldId,
                        NewValue = Id
                    });
            }           
            if (DisplayHookEvent != null)
            {
                var args = new DisplayHookArgs();
                args.Value = obj;
                DisplayHookEvent.Invoke(this, args);
                SetValue(TextProperty, args.Text);
            }
            else
            {
                SetValue(TextProperty, obj.FullName);
            }
        }

        private void OnSelected(SelectedEventArgs e)
        {
            this.Value = e.subjectObj;
            base.Text = No;
            e.Handled = true;
            //FocusEvent?.Invoke(this);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (base.IsReadOnly)
                return;
            if (e.Key == Key.Enter)
            {
                Keyboard.Press(Key.Tab);
            }
            else if (e.Key == Key.F7)
            {
                AccountSubjectPopup popupForm = new AccountSubjectPopup();
                popupForm.SelectedEvent += OnSelected;
                popupForm.ShowDialog();
            }
            else if (e.Key == Key.Escape || e.Key == Key.Subtract)
            {
                Value = new AccountSubjectObj();
                base.Text = "";
                e.Handled = true;
            }
        }

    }
}
