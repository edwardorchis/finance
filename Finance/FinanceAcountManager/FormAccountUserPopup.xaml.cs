using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.UI.FinanceDelegateEventHandler;

namespace FinanceAcountManager
{
    public partial class FormAccountUserPopup : Window
    {
        public FinanceMsgEventHandler FinanceMsgEvent;
        public FormAccountUserPopup()
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
                    case "ok":
                        Dictionary<string, string> paras = new Dictionary<string, string>();
                        paras["no"] = xNo;
                        paras["name"] = xName;
                        paras["pwd1"] = txtBoxPwd1.Password;
                        paras["pwd2"] = txtBoxPwd2.Password;
                        FinanceMsgEvent?.Invoke(0, paras);
                        Close();
                        break;
                    case "close":
                    case "cancel":                        
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

        public string xNo
        {
            get { return (string)GetValue(xNoProperty); }
            set { SetValue(xNoProperty, value); }
        }
        static readonly DependencyProperty xNoProperty =
            DependencyProperty.Register("xNo", typeof(string), typeof(FormAccountManagerPopup), new PropertyMetadata(""));

        public string xName
        {
            get { return (string)GetValue(xNameProperty); }
            set { SetValue(xNameProperty, value); }
        }

        static readonly DependencyProperty xNameProperty =
            DependencyProperty.Register("xName", typeof(string), typeof(FormAccountManagerPopup), new PropertyMetadata(""));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

    }

    


}
