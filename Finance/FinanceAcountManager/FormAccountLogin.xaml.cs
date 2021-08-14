using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.UI.FinanceDelegateEventHandler;

namespace FinanceAcountManager
{
    public partial class FormAccountLogin : Window
    {
        public FormAccountLogin()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var txt = (sender as Button).Name;
                switch (txt)
                {
                    case "okLogin":                       
                        var ret = CommondHandler.Process("Verification " + xLoginNo + " " + CryptInfoHelper.GetEncrypt(txtBoxLoginPwd1.Password));
                        if (ret != 0)
                        {
                            MessageBox.Show("密码错误");
                            return;
                        }
                        MainWindow mainWindow = new MainWindow();
                        App.Current.MainWindow = mainWindow;
                        this.Close();
                        //显示主窗体
                        mainWindow.Show();
                        break;
                    case "closeLogin":
                    case "cancelLogin":                        
                        Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());               
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public string xLoginNo
        {
            get { return (string)GetValue(xLoginNoProperty); }
            set { SetValue(xLoginNoProperty, value); }
        }
        static readonly DependencyProperty xLoginNoProperty =
            DependencyProperty.Register("xLoginNo", typeof(string), typeof(FormAccountManagerPopup), new PropertyMetadata(""));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }

    


}
