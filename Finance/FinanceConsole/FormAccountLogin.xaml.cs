using Finance.Account.Source;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.UI.FinanceDelegateEventHandler;

namespace FinanceConsole
{
    public partial class FormAccountLogin : Window
    {
        public FormAccountLogin()
        {
            var defaultConnectionString = ConfigHelper.XmlReadConnectionString("Finance.exe.config", "default");
            DBHelper.DefaultInstance = new DBHelper(defaultConnectionString);
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
                        var bSuc = AccountCtlMain.Verification(xLoginNo, txtBoxLoginPwd1.Password);
                        if (!bSuc)
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
            if (255 == CommondHandler.Test())
            {
                CommondHandler.Process("init");
            }
        }

    }

    


}
