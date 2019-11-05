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
    public partial class FormUserChangePasswordPopup : Window
    {
        public FormUserChangePasswordPopup()
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
                    case "save":                       
                        Save();                     
                        FinanceMessageBox.Info("修改成功");
                        break;
                    case "close":
                    case "exit":                  
                        
                        break;
                }
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void Save()
        {
            var oldpwd = txtOldPwd.Password;
            var newpwd1 = txtNewPwd1.Password;
            var newpwd2 = txtNewPwd2.Password;

            if (string.IsNullOrEmpty(oldpwd))
            {
                throw new Exception("旧密码不能为空");
            }
            if (string.IsNullOrEmpty(newpwd1))
            {
                throw new Exception("新密码不能为空");
            }
            if (string.IsNullOrEmpty(newpwd2))
            {
                throw new Exception("确认密码不能为空");
            }
            if (newpwd1 != newpwd2)
            {
                throw new Exception("新密码和确认密码不一致");
            }

            DataFactory.Instance.GetUserExecuter().ChangePassword(oldpwd, newpwd1);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
