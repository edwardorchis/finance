using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.UI;
using Finance.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FinanceClient
{
    public class LoginViewModel : ViewModelBase
    {
        #region 构造函数及登陆用户名及密码

        public LoginViewModel()
        {
           
        }

        #endregion

        #region 局部变量
        static ILogger logger = new DefaultLogger(typeof(FinanceForm));
        #endregion

        #region 成员
        private string userName;
        private string userPassword;
        private long tid;
        #endregion

        #region 属性
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get { return userName; } set { this.SetProperty(ref this.userName, value); } }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPassword { get { return userPassword; } set { this.SetProperty(ref this.userPassword, value); } }
      
        public long Tid { get { return tid; } set { this.SetProperty(ref this.tid, value); } }
        #endregion

        #region 工作方法
        /// <summary>
        /// 用于验证登陆信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>
        /// 返回值代表 
        /// -5：用户名或密码不正确
        /// -4：密码含有特殊字符 -3：密码不能为空
        /// -2：用户名不能为空
        /// -1：数据库未连接 0：登陆失败
        ///  1：登陆成功  2：
        /// 
        /// </returns>
        public int Verification()
        {
            int flag = 1;
            try
            {
                if (string.IsNullOrEmpty(this.userName))
                {
                    flag = -2;
                    return flag;
                }
                if (string.IsNullOrEmpty(this.userPassword))
                {
                    flag = -3;
                    return flag;
                }
   
                DataFactory.Instance.GetUserExecuter().Login(tid, userName, userPassword);

                FinanceControlEventsManager.Instance.GetAccountSubjectListEvent += () => {
                    List<AccountSubject> auxiliaries = DataFactory.Instance.GetAccountSubjectExecuter().List();             
                    var json = JsonConvert.SerializeObject(auxiliaries);
                    return JsonConvert.DeserializeObject<List<AccountSubjectObj>>(json);

                };
                FinanceControlEventsManager.Instance.GetAuxiliaryObjListEvent += () => {
                    List<Auxiliary> auxiliaries = DataFactory.Instance.GetAuxiliaryExecuter().List();
                    var json = JsonConvert.SerializeObject(auxiliaries);
                    return JsonConvert.DeserializeObject<List<AuxiliaryObj>>(json);
                };

                FinanceControlEventsManager.Instance.MessageEventHandlerEvent += (level, msg) =>
                {                    
                    switch (level)
                    {
                        case MessageLevel.ERR:
                            logger.Error(msg);
                            break;
                        case MessageLevel.INFO:
                            logger.Debug(msg);
                            break;
                        case MessageLevel.WARN:
                            logger.Warn(msg);
                            break;
                    }

                };
            }
            catch (Exception ex)
            {
                flag = -5;
                //出现错误无视
                Console.WriteLine(ex.Message);
            }
            return flag;
        }

        #endregion


    }

}
