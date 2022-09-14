using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace FinanceClient
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region 构造函数

        public LoginWindow()
        {
            this.Topmost = true;//设置为最顶层
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            this.Loaded += LoginWindow_Loaded;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bwLoad.DoWork += BwLoad_DoWork;
            bwLoad.RunWorkerCompleted += BwLoad_RunWorkerCompleted;
        }



        #endregion

        #region 属性

        #endregion

        #region 局部变量
        System.ComponentModel.BackgroundWorker bwLoad = new System.ComponentModel.BackgroundWorker
        {
            WorkerSupportsCancellation = true,//支持取消后台操作
        };

        LoginViewModel _viewmodel;

        ObservableCollection<UserInfo> listUserInfo;
        /// <summary>
        /// 生成保存登录信息的XML文件
        /// 同时获取XML文件数据
        /// </summary>
        LoginInfoXmlHelper loginInfoXmlHelper = new LoginInfoXmlHelper();
        /// <summary>
        /// 后台执行
        /// </summary>
        System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker
        {
            WorkerSupportsCancellation = true,//支持取消后台操作
        };
        /// <summary>
        /// 登陆信息的反馈值
        /// </summary>
        int feedBack = 0;
        /// <summary>
        /// 取消后台操作
        /// </summary>
        bool cancellationOperation = false;

        #endregion

        #region 界面元素操作
        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //App.Current.Shutdown();
            Environment.Exit(0);
        }
        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion

        #region 界面事件

        private void BwLoad_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (this.cancellationOperation)
            {
                this.cancellationOperation = false;//恢复以前状态        
                return;
            }
            
            //计时器 为登录框添加数据计时执行
            var timer1 = new System.Timers.Timer
            {
                Interval = 100,
            };
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(theout1);//到达时间的时候执行事件；
            timer1.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；   
            timer1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                                  //计时器 自动登录做准备的数据计时执行
            var timer2 = new System.Timers.Timer
            {
                Interval = 400,
            };
            timer2.Elapsed += new System.Timers.ElapsedEventHandler(theout2);
            timer2.AutoReset = false;
            timer2.Enabled = true;

            this.LoadGrid.Visibility = System.Windows.Visibility.Collapsed;//遮罩层隐藏
        }

        private void BwLoad_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var lst = DataFactory.Instance.GetAccountCtlExecuter().GetAccountList();
            Dictionary<long, string> tidSource = new Dictionary<long, string>();
            lst.ForEach(item => { tidSource.Add(item.id, string.Format("{0}|{1}", item.no, item.name)); });
            Action<ComboBox, Dictionary<long, string>> updateAction = new Action<ComboBox, Dictionary<long, string>>(SetTidItemSource);
            cmbTid.Dispatcher.BeginInvoke(updateAction, cmbTid, tidSource);

            listUserInfo = loginInfoXmlHelper.GetLoginInfo();
            Action<LoginWindow, ObservableCollection<UserInfo>> updateAction1 = new Action<LoginWindow, ObservableCollection<UserInfo>>(SetListUserInfoViewSource);
            this.Dispatcher.BeginInvoke(updateAction1, this, listUserInfo);            
        }

        private void SetTidItemSource(ComboBox tb, Dictionary<long, string> tidSource)
        {
            cmbTid.ItemsSource = tidSource;
        }

        private void SetListUserInfoViewSource(LoginWindow window, ObservableCollection<UserInfo> listUserInfo)
        {
            CollectionViewSource ListUserInfoViewSource = (CollectionViewSource)this.FindResource("ListUserInfoViewSource");
            ListUserInfoViewSource.Source = listUserInfo;
        }

        /// <summary>
        /// 登陆后台运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) =>
            //反馈
            feedBack = _viewmodel.Verification();
        /// <summary>
        /// 登陆操作后台运行结束后操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bw_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            //取消后台登录
            if (this.cancellationOperation)
            {
                this.cancellationOperation = false;//恢复以前状态
                return;//截断
            }

            switch (feedBack)
            {
                case 1:
                    //登录成功
                    //获取新的登录信息
                    var userInfo = new UserInfo
                    {
                        AutomaticLogon = (bool)this.checkBox2.IsChecked,
                        RememberPwd = (bool)this.checkBox1.IsChecked,
                        UserName = this.cmb1.Text.Trim(),
                        UserPwd = this.passwordBox1.Password.Trim(),
                        Tid = (long)this.cmbTid.SelectedValue
                    };
                    var accountName = cmbTid.Text;
                    if (!(bool)this.checkBox1.IsChecked)
                    {
                        userInfo.UserPwd = "";
                    }
                    listUserInfo.Remove(listUserInfo.FirstOrDefault(u => u.UserName == userInfo.UserName));
                    listUserInfo.Insert(0, userInfo);
                    //保存登录信息
                    loginInfoXmlHelper.CreateXml(listUserInfo.ToList());
                    //生成主窗体
                    FormMain mainWindow = new FormMain();
                    //设置系统主窗体
                    App.Current.MainWindow = mainWindow;
                    //关闭登陆界面
                    this.Close();
                    //this.DialogResult = true;
                    mainWindow.Title = string.Format("Finance - [{0}]", accountName);
                    //显示主窗体
                    mainWindow.Show();
                    break;
                case 0:
                    MessageBox.Show("登录失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case -1:
                    System.Windows.MessageBox.Show("数据库未连接！", "系统提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    break;
                case -2:
                    System.Windows.MessageBox.Show("用户名不能为空！", "系统提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    break;
                case -3:
                    System.Windows.MessageBox.Show("密码不能为空！", "系统提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    break;
                case -4:
                    System.Windows.MessageBox.Show("密码含有特殊字符！", "系统提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    break;
                case -5:
                    System.Windows.MessageBox.Show("用户名或密码不正确！", "系统提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    break; 
                default:
                    MessageBox.Show("未知错误！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
            this.LoadGrid.Visibility = System.Windows.Visibility.Collapsed;//遮罩层隐藏
        }


        /// <summary>
        /// 窗体加载完成后操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewmodel == null)
                {
                    //界面中浮云移动动画
                    //Storyboard sbd = Resources["sbCloud"] as Storyboard;
                    //sbd.Begin();

                    _viewmodel = new LoginViewModel();
                    this.DataContext = _viewmodel;


                    //计时器 为登录框添加数据计时执行
                    var timer1 = new System.Timers.Timer
                    {
                        Interval = 100,
                    };
                    timer1.Elapsed += new System.Timers.ElapsedEventHandler(theout1);//到达时间的时候执行事件；
                    timer1.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；   
                 
                    var timer2 = new System.Timers.Timer
                    {
                        Interval = 400,
                    };
                    timer2.Elapsed += new System.Timers.ElapsedEventHandler(theout2);
                    timer2.AutoReset = false;    

                    this.LoadGrid.Visibility = System.Windows.Visibility.Visible;//遮罩层可见
                    bwLoad.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 为登录框添加数据计时执行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void theout1(object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                //显示上次登录
                this.cmb1.SelectedIndex = 0;
            }));
        }
        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void theout2(object source, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                //显示上次登录
                var userLogin = listUserInfo.FirstOrDefault();
                if (userLogin != null)
                {
                    if (userLogin.AutomaticLogon)//自动登录
                    {
                        //触发登录
                        this.btnLogin.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, this.btnLogin));
                    }
                }
            }));
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("FinanceAcountManager.exe");
                process.StartInfo = startInfo;
                process.Start();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LoadGrid.Visibility = System.Windows.Visibility.Visible;//遮罩层可见
                bw.RunWorkerAsync();
            }
            catch (Exception)
            {
                //报错无视
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbkRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var window = new RegistrationWindow();
            //window.Show();
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("您确定要删除此登录信息吗？", "系统提示", MessageBoxButton.YesNo, MessageBoxImage.Information))
            {
                var btn = sender as Button;
                string userName = btn.DataContext.ToString();
                listUserInfo.Remove(listUserInfo.FirstOrDefault(u => u.UserName == userName));
                loginInfoXmlHelper.CreateXml(listUserInfo.ToList());//保存登录信息
            }
        }

        /// <summary>
        /// 下拉框文本输入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox && _viewmodel != null)
            {
                var cmb = sender as ComboBox;
                var userinfo = listUserInfo.FirstOrDefault(u => u.UserName == cmb1.Text);
                if (userinfo == null)
                {
                    this.passwordBox1.Password = "";
                    this.checkBox1.RaiseEvent(new RoutedEventArgs(CheckBox.UncheckedEvent, this.checkBox1));
                    this.cmbTid.SelectedIndex = 0;
                }
                else
                {
                    UserInfoRecond(userinfo);
                }
            }
        }
        /// <summary>
        /// 记录UserInfo信息
        /// </summary>
        /// <param name="userinfo"></param>
        private void UserInfoRecond(UserInfo userinfo)
        {
            this.cmb1.Text = userinfo.UserName;
            this.passwordBox1.Password = userinfo.UserPwd;
            this.cmbTid.SelectedValue = userinfo.Tid;
            this.checkBox1.RaiseEvent(new RoutedEventArgs(CheckBox.UncheckedEvent, this.checkBox1));
            if (userinfo.AutomaticLogon)
            {
                this.checkBox2.RaiseEvent(new RoutedEventArgs(CheckBox.CheckedEvent, this.checkBox2));
            }
            else if (!userinfo.RememberPwd)
            {
                this.checkBox1.RaiseEvent(new RoutedEventArgs(CheckBox.UncheckedEvent, this.checkBox1));
            }
            else if (userinfo.RememberPwd)
            {
                this.checkBox1.RaiseEvent(new RoutedEventArgs(CheckBox.CheckedEvent, this.checkBox1));
            }
        }


        #endregion

        #region Load登录加载界面

        #region 登陆加载界面局部变量
        /// <summary>
        /// 队列计时器
        /// </summary>
        private DispatcherTimer animationTimer;
        #endregion

        #region 登陆加载界面初始化方法

        public void LoadingWait()
        {
            animationTimer = new DispatcherTimer(
                DispatcherPriority.ContextIdle, Dispatcher);
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 90);
        }
        #endregion

        #region 执行方法

        private void Start()
        {
            animationTimer.Tick += HandleAnimationTick;
            animationTimer.Start();
        }

        private void Stop()
        {
            animationTimer.Stop();
            animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);
        }

        private void SetPosition(Ellipse ellipse, double offset,
            double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0
                + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50
                + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;
            if (animationTimer == null)
            {
                LoadingWait();
            }
            if (isVisible)
                Start();
            else
                Stop();
        }

        /// <summary>
        /// 取消登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.cancellationOperation = true;
            if(bw.IsBusy)
                bw.CancelAsync();//取消后台操作
            if (bwLoad.IsBusy)
                bwLoad.CancelAsync();//取消后台操作
            this.LoadGrid.Visibility = System.Windows.Visibility.Collapsed;//遮罩层隐藏
        }
        #endregion

        #endregion

    }

    /// <summary>
    /// 用户类用于保存记录
    /// </summary>
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public long Tid { set; get; }
        /// <summary>
        /// 是否自动登录
        /// </summary>
        //[System.ComponentModel.DefaultValue(false)]
        public bool AutomaticLogon { get; set; }
        /// <summary>
        /// 是否记住密码
        /// </summary>
        //[System.ComponentModel.DefaultValue(false)]
        public bool RememberPwd { get; set; }
        
    }


    /// <summary>
    /// Xml配置文件
    /// </summary>
    public class LoginInfoXmlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        public LoginInfoXmlHelper()
        {
            string appStartPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            StringBuilder builder = new StringBuilder();
            builder.Append(appStartPath);
            builder.Append("\\AthenFile\\");
            builder.Append("\\XML\\");
            System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(builder.ToString());
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();//创建一个
            }
            builder.Append("LoginInfoXml.xml");
            this.fullFilePath = builder.ToString();
        }

        #region 成员
        /// <summary>
        /// 文件全路径
        /// </summary>
        string fullFilePath;
        #endregion

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<UserInfo> GetLoginInfo()
        {
            var loginInfo = new List<UserInfo>();
            if (InfoFile.Exists)
            {
                XmlSerializer xml = new XmlSerializer(loginInfo.GetType());
                using (Stream s = InfoFile.OpenRead())
                {
                    loginInfo = xml.Deserialize(s) as List<UserInfo>;
                }
            }
            var tList = new ObservableCollection<UserInfo>();
            foreach (var item in loginInfo)
            {
                tList.Add(new UserInfo
                {
                    AutomaticLogon = item.AutomaticLogon,
                    RememberPwd = item.RememberPwd,
                    UserName = CryptInfoHelper.GetDecrypte(item.UserName),
                    UserPwd = CryptInfoHelper.GetDecrypte(item.UserPwd),
                    Tid = item.Tid
                });
            }
            return tList;
        }

        /// <summary>
        /// 文件方式
        /// </summary>
        private FileInfo InfoFile
        {
            get
            {
                return new FileInfo(fullFilePath);
            }
        }

        /// <summary>
        /// 生成Xml文件
        /// </summary>
        /// <param name="loginInfo"></param>
        private void Create_Xml(List<UserInfo> loginInfo)
        {
            var tList = new List<UserInfo>();
            foreach (var item in loginInfo)
            {
                tList.Add(new UserInfo
                {
                    AutomaticLogon = item.AutomaticLogon,
                    RememberPwd = item.RememberPwd,
                    UserName = CryptInfoHelper.GetEncrypt(item.UserName),
                    UserPwd = CryptInfoHelper.GetEncrypt(item.UserPwd),
                    Tid = item.Tid
                });
            }
            XmlSerializer xmls = new XmlSerializer(tList.GetType());
            if (InfoFile.Exists)
            {
                InfoFile.Delete();
            }
            using (Stream s = InfoFile.OpenWrite())
            {
                xmls.Serialize(s, tList);
                //s.Close();
            }
        }

        /// <summary>
        /// 生成Xml文件
        /// </summary>
        public void CreateXml(List<UserInfo> loginInfo)
        {
            Create_Xml(loginInfo);
        }

    }
}
