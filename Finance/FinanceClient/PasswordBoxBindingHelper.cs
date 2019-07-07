using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceClient
{
    public static class PasswordBoxBindingHelper
    {

        public static bool GetIsPasswordBindingEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPasswordBindingEnabledProperty);
        }

        public static void SetIsPasswordBindingEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPasswordBindingEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPasswordBindingEnabledProperty = DependencyProperty.RegisterAttached("IsPasswordBindingEnabled", typeof(bool),
            typeof(PasswordBoxBindingHelper), new UIPropertyMetadata(false, OnIsPasswordBindingEnabledChanged));

        private static void OnIsPasswordBindingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = obj as System.Windows.Controls.PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
                }
            }
        }



        static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (System.Windows.Controls.PasswordBox)sender;
            if (!String.Equals(GetBindedPassword(passwordBox), passwordBox.Password))
            {
                SetBindedPassword(passwordBox, passwordBox.Password);
                //目的是让打字的光标显示在最后
                int length = passwordBox.Password.Length;
                SetPasswordBoxSelection(passwordBox, length, length);
            }
        }

        public static string GetBindedPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BindedPasswordProperty);
        }

        public static void SetBindedPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BindedPasswordProperty, value);
        }

        public static readonly DependencyProperty BindedPasswordProperty = DependencyProperty.RegisterAttached("BindedPassword", typeof(string),
            typeof(PasswordBoxBindingHelper), new UIPropertyMetadata(string.Empty, OnBindedPasswordChanged));

        private static void OnBindedPasswordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = obj as System.Windows.Controls.PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.Password = e.NewValue == null ? string.Empty : e.NewValue.ToString();
            }
        }

        /// <summary>
        /// 在更改了密码框的密码后, 需要手动更新密码框插入符(CaretIndex)的位置, 可惜的是, 
        /// 密码框并没有给我们提供这样的属性或方法(TextBox有, PasswordBox没有), 
        /// 可以采用下面的方法来设置
        /// </summary>
        /// <param name="passwordBox"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private static void SetPasswordBoxSelection(System.Windows.Controls.PasswordBox passwordBox, int start, int length)
        {
            var select = passwordBox.GetType().GetMethod("Select",
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            select.Invoke(passwordBox, new object[] { start, length });
        }


    }
}
