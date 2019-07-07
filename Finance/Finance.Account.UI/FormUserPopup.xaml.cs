using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Newtonsoft.Json;
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
using System.Windows.Shapes;
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormAccountSubjectPopup.xaml 的交互逻辑
    /// </summary>
    public partial class FormUserPopup : Window
    {
        public AfterSaveEventHandler AfterSaveEvent;

        string _origionUserName = "";
        string _origionPassword = "******";
        public FormUserPopup()
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
                    case "savenew":
                        if (NeedSave())
                        {
                            Save();
                        }
                        else
                        {
                            Console.WriteLine("don't change,no need save.");
                        }
                        UserName = "";
                        _origionUserName = "";
                        txtPwd.Password = _origionPassword;
                        UserId = 0;
                        break;  
                    case "save":
                        if (NeedSave())
                        {
                            Save();
                        }
                        else
                        {
                            Console.WriteLine("don't change,no need save.");
                        }
                        FinanceMessageBox.Info("保存成功");
                        break;
                    case "close":
                    case "exit":
                        if (NeedSave())
                        {
                            MessageBoxResult ret = FinanceMessageBox.Quest("修改了用户，需要进行保存吗？");
                            if (ret == MessageBoxResult.Yes)
                            {
                                Save();
                            }
                            else if (ret == MessageBoxResult.Cancel)
                                break;
                        }
                        Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void Save()
        {
            UserId = DataFactory.Instance.GetUserExecuter().Save(UserId, UserName,Password);
            AfterSaveEvent?.Invoke();
        }

        bool NeedSave()
        {
            bool result = false;
            result = _origionUserName != UserName
                || _origionPassword != Password;
            return result;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public long UserId
        {
            set;get;
        }

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(FormUserPopup), new PropertyMetadata(""));

        string Password
        {
            get {
                return txtPwd.Password;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtPwd.Password = "******";
            _origionUserName = UserName;
        }

    }

    


}
