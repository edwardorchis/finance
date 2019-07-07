using Finance.Account.Controls.Commons;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Keyboard = Finance.Account.Controls.Commons.Keyboard;

namespace Finance.Account.Controls
{
    /// <summary>
    /// AccountSubjectBox.xaml 的交互逻辑
    /// </summary>
    public partial class AuxiliaryBox : TextBox
    {       
        public AuxiliaryBox()
        {
            InitializeComponent();
            base.TextWrapping = TextWrapping.WrapWithOverflow;
            base.KeyDown += OnKeyDown;
            base.MouseDoubleClick += box_MouseDoubleClick;
            base.LostFocus += OnLostFocus;
            base.GotFocus += OnGotFocus;
        }

        private void box_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AuxiliaryPopup popupForm = new AuxiliaryPopup(AuxiliaryType.AccountContent);
            popupForm.SelectedEvent += OnSelected;
            popupForm.ShowDialog();
        }
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (base.IsReadOnly)
                return;            
            base.Background = Consts.HIGHLIGHT_BRUSH;
           
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (base.IsReadOnly)
                return;
            var txt = base.Text;
            var obj = AuxiliaryList.FindByNo(AuxiliaryType.AccountContent, txt.Trim());
            if (obj != null)
                base.Text = string.IsNullOrEmpty(obj.name) ? txt:obj.name ;         
            base.Background = Consts.WHITE_BRUSH;
            
        }
        private void OnSelected(AuxiliaryObj obj)
        {
            if (base.IsReadOnly)
                return;
            base.Text = obj.name;
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
                AuxiliaryPopup popupForm = new AuxiliaryPopup(AuxiliaryType.AccountContent);
                popupForm.SelectedEvent += OnSelected;
                popupForm.ShowDialog();
            }
            else if (e.Key == Key.Escape || e.Key == Key.Subtract)
            {                
                base.Text = "";
                e.Handled = true;
            }
        }



    }
}
