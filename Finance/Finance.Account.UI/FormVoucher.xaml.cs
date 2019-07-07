using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Finance.Account.UI.Model;
using Finance.Utils;
using Finance.UI;
using Finance.Account.Data.Model;
using System.Linq;
using System.Windows.Media;

namespace Finance.Account.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FormVoucher : FinanceForm
    {
        protected static ILogger logger = new DefaultLogger(typeof(FormVoucher));
        private long mid = 0;
        Dictionary<string, Dictionary<string, object>> mUserDefineDataSource = new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// 窗口切换的时候禁止Load
        /// </summary>
        private bool isShow = false;

        public long VoucherId { set { mid = value; isShow = false; } get { return mid; } }

        public FormVoucher()
        {            
            InitializeComponent();
            txtBBillNo.KeyDown += TxtBBillNo_KeyDown;
            cmbWords.SelectionChanged += (sender1, e1) => { RestSerialNo(); };
            dateDate.SelectedDateChanged += (sender1, e1) => { RestSerialNo(); };
            voucherGrid.AccountSubjectChangeEvent += VoucherGrid_AccountSubjectChangeEvent;
            voucherGrid.CellKeyDownEvent += (sender, e) => {
                if (sender is AccountSubjectBox)
                {
                    if (e.Key == System.Windows.Input.Key.F8)
                    {
                        var currentRowIndex = voucherGrid.CurrentRowIndex;
                        if (currentRowIndex == -1)
                        {
                            var box = sender as AccountSubjectBox;
                            currentRowIndex = int.Parse(box.Name.Substring(box.Name.IndexOf("_") + 1));
                        }
                        var voucherGridDataSource = voucherGrid.DataSource[currentRowIndex];
                        DataChangedArgs args = new DataChangedArgs();
                        args.Name = voucherGridDataSource.UniqueKey;
                        args.NewValue = voucherGridDataSource.AccountSubjectId;
                        VoucherGrid_AccountSubjectChangeEvent(sender, args);
                    }
                }
            };

            voucherGrid.MessageEvent += (level, msg) =>
            {
                MessageInfo(level, msg);
            };
            voucherGrid.RowChangeEvent += (sender, e) =>
            {
                LoadUdefPanel(e.NewKey);
            };
            voucherGrid.BeginLayoutEvent += () =>
            {
                mVoucherGridLayouting = true;
            };
            voucherGrid.EndLayoutEvent += () =>
            {
                mVoucherGridLayouting = false;
            };
        }

        bool mVoucherGridLayouting = false;

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mVoucherGridLayouting)
                    return;
                var txt = (sender as Button).Name;
                logger.Debug("btn_Click[{0}]",txt);
                switch (txt)
                {
                    case "new":
                        New();
                        break;
                    case "append":
                        voucherGrid.AppendRow();
                        break;
                    case "insert":
                        voucherGrid.InsertRow(voucherGrid.CurrentRowIndex);
                        break;
                    case "delete":
                        voucherGrid.DeleteRow(voucherGrid.CurrentRowIndex);
                        break;                   
                    case "save":
                        var v = ReadVoucher();
                        v.header.id = mid;
                        mid = DataFactory.Instance.GetVoucherExecuter().Save(v);
                        New();
                        //LoadVoucher(mid);
                        break;
                    case "check":
                        DataFactory.Instance.GetVoucherExecuter().Check(mid);
                        LoadVoucher(mid);
                        break;
                    case "uncheck":
                        DataFactory.Instance.GetVoucherExecuter().UnCheck(mid);
                        LoadVoucher(mid);
                        break;
                    case "cancel":
                        DataFactory.Instance.GetVoucherExecuter().Cancel(mid);
                        LoadVoucher(mid);
                        break;
                    case "uncancel":
                        DataFactory.Instance.GetVoucherExecuter().UnCancel(mid);
                        LoadVoucher(mid);
                        break;
                    case "previous":
                        LoadVoucher(mid,LINKED.PREVIOUS);
                        break;
                    case "next":
                        LoadVoucher(mid, LINKED.NEXT);
                        break;
            
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FinanceMessageBox.Error(ex.Message);
            }
        }

        void New()
        {
            logger.Debug("New");
            mid = 0;
            Init();
            voucherGrid.AppendRow();
        }

        void RestSerialNo()
        {
            DateTime date =(DateTime) dateDate.SelectedDate;
            var exKey = string.Format("{0}_{1}_{2}", date.Year, date.Month, cmbWords.Text);

            lblPeriod.Content = string.Format("{0}年第{1}期", date.Year, date.Month);
            intNo.Value = (int)DataFactory.Instance.GetSerialNoExecuter().Get(SerialNoKey.VoucherNo, exKey);
        }

        void Init()
        {
            try
            {
                UnLock();
                lblStatus.Visibility = Visibility.Hidden;

                txtAgent.Text = "";
                txtCashier.Text = "";
                txtReference.Text = "";
                lblChecker.Tag = null;
                lblChecker.Content = null;
                lblPoster.Tag = null;
                lblPoster.Content = null;

                var now = DateTime.Now;
                var year = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentYear);
                var period = DataFactory.Instance.GetSystemProfileExecuter().GetInt(SystemProfileKey.CurrentPeriod);

                if (now.Year == year && now.Month == period)
                { }
                else
                    now = CommonUtils.CalcMaxPeriodDate(year, period);
                dateBusinessDate.SelectedDate = now;
                dateDate.SelectedDate = now;
                lblPeriod.Content = string.Format("{0}年第{1}期", now.Year, now.Month);

                List<string> list = new List<string>();
                List<Auxiliary> auxiliaries = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.ProofOfWords);
                auxiliaries.ForEach(a =>
                {
                    list.Add(a.name);
                });
                cmbWords.ItemsSource = list;
                cmbWords.SelectedIndex = 0;

                intSn.Value = (int)DataFactory.Instance.GetSerialNoExecuter().Get(SerialNoKey.VoucherSn);
                lblCreater.Tag= DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.UserId);
                lblCreater.Content = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.UserName);
                logger.Debug("Init::Clear");
                voucherGrid.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        Voucher ReadVoucher()
        {
            VoucherHeader header = new VoucherHeader();
            header.agent = txtAgent.Text;
            header.businessDate = DateTime.Parse(dateBusinessDate.Text);
            header.cashier = txtCashier.Text;
            header.creater = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.UserId);
            header.creatTime = DateTime.Now;
            header.date = DateTime.Parse(dateDate.Text); ;
            header.word = cmbWords.Text;
            header.serialNo = int.Parse(intSn.Value.ToString());
            header.reference = txtReference.Text;
            header.no = int.Parse(intNo.Value.ToString());
            header.year = header.date.Year;
            header.period = header.date.Month;
            header.id = 0;

            var lst = voucherGrid.DataSource; 

            int i = 1;
            List<VoucherEntry> entries = new List<VoucherEntry>();
            Dictionary<string, Dictionary<string, object>> userDefineValues = new Dictionary<string, Dictionary<string, object>>();
            foreach (var item in lst)
            {
                var entry = new VoucherEntry();
                entry.index = i;
                entry.explanation = item.Content;
                entry.accountSubjectId = item.AccountSubjectId;
                entry.accountSubjectNo = item.AccountSubjectNo;
                if (item.DebitsAmount > 0)
                {
                    entry.amount = item.DebitsAmount;
                    entry.direction = 1;
                }
                else
                {
                    entry.amount = item.CreditAmount;
                    entry.direction = -1;
                }
                if (entry.accountSubjectId == 0 && entry.amount == 0)
                    continue;
                entry.uniqueKey = item.UniqueKey;
                entries.Add(entry);

                if (mUserDefineDataSource.ContainsKey(item.UniqueKey))
                {
                    userDefineValues.Add(item.UniqueKey, mUserDefineDataSource[item.UniqueKey]);
                }
                i++;
            }

            Voucher v = new Voucher() { header = header,entries=entries,udefenties = userDefineValues };
            return v;
        }

        void LoadVoucher(long id,LINKED linked = LINKED.CURRENT)
        {
            Voucher v = DataFactory.Instance.GetVoucherExecuter().Find(id, linked);
            VoucherHeader header = v.header;
            if (v.udefenties != null)
                mUserDefineDataSource = v.udefenties;
            mid = header.id;

            lblStatus.Visibility = Visibility.Hidden;
            UnLock();

            txtBBillNo.Text = "";
            txtAgent.Text = header.agent;
            txtCashier.Text = header.cashier;
            txtReference.Text = header.reference;

            lblCreater.Tag = header.creater;
            lblCreater.Content = DataFactory.Instance.GetUserExecuter().FindName(header.creater);

            dateBusinessDate.SelectedDate=header.businessDate;
            dateDate.SelectedDate = header.date;
            cmbWords.SelectedValue = header.word;
            intSn.Value =(int) header.serialNo;
            intNo.Value = (int)header.no;
            
            lblPeriod.Content = string.Format("{0}年第{1}期", header.date.Year, header.date.Month);

            List<VoucherGridItem> items = new List<VoucherGridItem>();
            v.entries.ForEach(entry=> {
                var item = new VoucherGridItem();
                item.AccountSubjectId = entry.accountSubjectId;
                item.Content = entry.explanation;
                if (entry.direction == 1)
                {
                    item.DebitsAmount = entry.amount;
                }
                else
                {
                    item.CreditAmount = entry.amount;
                }
                item.UniqueKey = entry.uniqueKey;                
                items.Add(item);
            });
            voucherGrid.DataSource = items;
            logger.Debug("LoadVoucher:[{0}]", items.Count);
            voucherGrid.Refresh();

            lblChecker.Tag = header.checker;
            lblChecker.Content = DataFactory.Instance.GetUserExecuter().FindName(header.checker);

            lblPoster.Tag = header.poster;
            lblPoster.Content = DataFactory.Instance.GetUserExecuter().FindName(header.poster);            
           
            if (header.status > 0)
            {
                ShowStatusLable((VoucherStatus)(header.status));
                Lock();
            }
          
        }

        void ShowStatusLable(VoucherStatus status)
        {
            if (Constant.VoucherStatusDictionary.ContainsKey(status))
            {
                lblStatus.Content = Constant.VoucherStatusDictionary[status];
                lblStatus.Visibility = Visibility.Visible;
            }
        }

        void Lock()
        {
            txtBBillNo.IsReadOnly = true;
            voucherGrid.IsReadOnly = true;
            txtAgent.IsReadOnly = true;
            txtCashier.IsReadOnly = true;
            txtReference.IsReadOnly = true;

            dateBusinessDate.IsEnabled = false;
            dateDate.IsEnabled = false;

            cmbWords.IsEnabled = false;

            intSn.IsReadOnly = true;
            intNo.IsReadOnly = true;
        }

        void UnLock()
        {
            txtBBillNo.IsReadOnly = false;
            voucherGrid.IsReadOnly = false;
            txtAgent.IsReadOnly = false;
            txtCashier.IsReadOnly = false;
            txtReference.IsReadOnly = false;

            dateBusinessDate.IsEnabled = true;
            dateDate.IsEnabled = true;

            cmbWords.IsEnabled = true;

            intSn.IsReadOnly = false;
            intNo.IsReadOnly = false;         
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isShow)
                return;
            isShow = true;
            voucherGrid.Load();
            Init();

            if (mid > 0)
                LoadVoucher(mid);
        }

        private void VoucherGrid_AccountSubjectChangeEvent(object sender, DataChangedArgs e)
        {            
            var accountSubjectBox = sender as AccountSubjectBox;
            if (accountSubjectBox == null)
                return;

            if ((accountSubjectBox.Value.flag & (int)AccountFlag.FLAG_USDEF_MASK) != 0)
            {
                FormVoucherUserDefinePopup popup = new FormVoucherUserDefinePopup(this, e);
                if (mUserDefineDataSource.ContainsKey(e.Name))
                    popup.DataSource = mUserDefineDataSource[e.Name];
                popup.UserDefinePopupEvent += (obj, args) =>
                {
                    if (args.Cancel)
                    {
                        (obj as FormVoucherUserDefinePopup).Close();
                        return;
                    }
                    if (mUserDefineDataSource.ContainsKey(args.Name))
                        mUserDefineDataSource[args.Name] = (Dictionary<string, object>)(args.OldValue);
                    else
                        mUserDefineDataSource.Add(args.Name, (Dictionary<string, object>)(args.OldValue));

                    var voucherGridDataSource = voucherGrid.DataSource;
                    var voucherGridCurrentRowIndex = voucherGrid.CurrentRowIndex;
                    if (voucherGrid.CurrentRowIndex < 0 || voucherGrid.CurrentRowIndex > voucherGridDataSource.Count)
                        return;
                    voucherGridDataSource[voucherGrid.CurrentRowIndex].DebitsAmount = (decimal)(args.NewValue);
                    (obj as FormVoucherUserDefinePopup).Close();
                    logger.Debug("VoucherGrid_AccountSubjectChangeEvent::UserDefinePopupEvent:[{0}]", voucherGridDataSource.Count);
                    voucherGrid.DataSource = voucherGridDataSource;
                    voucherGrid.Refresh();
                    //Keyboard.Press(System.Windows.Input.Key.Tab);
                };
                popup.ShowDialog();
            }
            else
            {
                if (mUserDefineDataSource.ContainsKey(e.Name))
                    mUserDefineDataSource.Remove(e.Name);
            }
        }

        public override void Closing(CancelEventArgs e)
        {
            //e.Cancel = true;
        }

        public override void OnSizeChanged(SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                scrollViewer.Height = e.NewSize.Height - 120;
            }
        }

        void MessageInfo(MessageLevel level, string msg)
        {
            switch (level)
            {
                case MessageLevel.INFO:
                    infoBox.Background = Brushes.LightGray;
                    break;
                case MessageLevel.WARN:
                    infoBox.Background = Brushes.Yellow;
                    break;
                case MessageLevel.ERR:
                    infoBox.Background = Brushes.Red;
                    break;
            }            
            infoBox.Text = msg;
        }



        void LoadUdefPanel(string UniqueKey)
        {
            if (!mUserDefineDataSource.ContainsKey(UniqueKey))
            {
                userDefinePanel.DataSource = new List<UserDefineInputItem>();
                return;
            }
            var udefDataDictionary = mUserDefineDataSource[UniqueKey];
            var lst = DataFactory.Instance.GetTemplateExecuter().GetUdefTemplate("_VoucherEntryUdef");
            if (lst == null)
                return;

            var voucherGridDataSource = voucherGrid.DataSource;
            var accountSubjectId = voucherGridDataSource.FirstOrDefault(d=>d.UniqueKey == UniqueKey).AccountSubjectId;
            var accountSubjectObj = AccountSubjectList.Find(accountSubjectId);           
            var accountFlagList = AuxiliaryList.Get(Controls.Commons.AuxiliaryType.AccountFlag);
            List<UserDefineInputItem> mUserDefineInputItems = new List<UserDefineInputItem>();
            foreach (var item in lst)
            {
                var flagLabel = item.tagLabel;
                if (!string.IsNullOrEmpty(flagLabel))
                {
                    var s = flagLabel.Split('|');
                    if (s.Length > 1)
                    {
                        var f = s[1];
                        var bF = false;
                        foreach (var F in accountFlagList)
                        {
                            var mask = 0;
                            if (!int.TryParse(F.name, out mask))
                                continue;
                            if (f == F.no && (accountSubjectObj.flag & mask) == 0)
                            {
                                bF = true;
                                break;
                            }
                        }
                        if (bF)
                            continue;
                    }
                }

                object val = item.defaultVal;
                if (udefDataDictionary != null)
                {
                    if (udefDataDictionary.ContainsKey(item.name))
                        val = udefDataDictionary[item.name];
                }
                mUserDefineInputItems.Add(new UserDefineInputItem
                {
                    Label = item.label,
                    DataType = CommonUtils.ConvertDataTypeFromStr(item.dataType),
                    Name = item.name,
                    DataValue = val,
                    TabIndex = item.tabIndex,
                    TagLabel = string.IsNullOrEmpty(item.tagLabel) ? "" : item.tagLabel,
                    Width = item.width
                });
            }

            mUserDefineInputItems = mUserDefineInputItems.OrderBy(item => item.TabIndex).ToList();
            userDefinePanel.DataSource = mUserDefineInputItems;
        }


        private void TxtBBillNo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                try
                {
                    if (string.IsNullOrEmpty(txtBBillNo.Text))
                        return;
                    e.Handled = true;
                    var taskId = DataFactory.Instance.GetInterfaceExecuter().ExecTask(ExecTaskType.CreateVoucher, "sp_generatevoucherbybillno", new Dictionary<string, object> { { "billNo", txtBBillNo.Text } });
                    var taskResult = DataFactory.Instance.GetInterfaceExecuter().GetTaskResult(taskId);
                    if (taskResult == null)
                    {
                        FinanceMessageBox.Error("生成结果为空，请联系工程师处理。");
                        return;
                    }
                    if (taskResult.status == 1)
                    {
                        var lst = DataFactory.Instance.GetVoucherExecuter().List(new Dictionary<string, object> { { "linkNo", txtBBillNo.Text } });
                        if (lst != null && lst.Count > 0)
                        {
                            mid = lst.FirstOrDefault().header.id;
                            LoadVoucher(mid);
                        }
                        else
                            FinanceMessageBox.Error("未找到关联的凭证，请联系工程师处理。");
                    }
                    else
                    {
                        FinanceMessageBox.Error(taskResult.result);
                    }
                }
                catch (Exception ex)
                {
                    FinanceMessageBox.Error(ex.Message);
                }
            }
        }
    }
}
