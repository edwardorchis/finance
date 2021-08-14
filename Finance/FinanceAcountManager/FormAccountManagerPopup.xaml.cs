using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.UI.FinanceDelegateEventHandler;

namespace FinanceAcountManager
{
    public partial class FormAccountManagerPopup : Window
    {
        public FinanceMsgEventHandler FinanceMsgEvent;
        public FormAccountManagerPopup()
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
                    case "okAcct":
                        Dictionary<string, string> paras = new Dictionary<string, string>();
                        paras["no"] = xAcctNo;
                        paras["name"] = xAcctName;
                        FinanceMsgEvent?.Invoke(0, paras);
                        Close();
                        break;
                    case "closeAcct":
                    case "cancelAcct":                        
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

        public string xAcctNo
        {
            get { return (string)GetValue(xAcctNoProperty); }
            set { SetValue(xAcctNoProperty, value); }
        }
        static readonly DependencyProperty xAcctNoProperty =
            DependencyProperty.Register("xAcctNo", typeof(string), typeof(FormAccountManagerPopup), new PropertyMetadata(""));

        public string xAcctName
        {
            get { return (string)GetValue(xAcctNameProperty); }
            set { SetValue(xAcctNameProperty, value); }
        }

        static readonly DependencyProperty xAcctNameProperty =
            DependencyProperty.Register("xAcctName", typeof(string), typeof(FormAccountManagerPopup), new PropertyMetadata(""));


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

    }

    


}
