using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.UI.FinanceDelegateEventHandler;

namespace FinanceAcountManager
{
    public partial class FormServiceManager : Window
    {
        public FormServiceManager()
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
                        FormAccountLogin lgnWindow = new FormAccountLogin();
                        this.Close();
                        lgnWindow.Show();
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
  
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }

    


}
