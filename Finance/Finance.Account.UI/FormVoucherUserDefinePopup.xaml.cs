using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.Account.UI.Model;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static Finance.Account.Controls.Commons.Consts;

namespace Finance.Account.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    internal partial class FormVoucherUserDefinePopup : Window
    {
        readonly System.Globalization.CultureInfo Culinfo = System.Globalization.CultureInfo.CreateSpecificCulture("zh-Cn");

        public DataChangedEventHandler UserDefinePopupEvent;

        Dictionary<string, decimal> mSumAmount = new Dictionary<string, decimal>();
        List<UserDefineInputItem> mUserDefineInputItems = new List<UserDefineInputItem>();

        AccountSubjectObj mAccountSubjectObj;
        DataChangedArgs mAccountSubjectChangeArgs;
        FormVoucher mFormVoucher;
        public FormVoucherUserDefinePopup(FormVoucher owner, DataChangedArgs args)
        {
            InitializeComponent();
            mAccountSubjectChangeArgs = args;
            mAccountSubjectObj = AccountSubjectList.Find((long)(args.NewValue));
            mFormVoucher = owner;
        }     

        public Dictionary<string,object> DataSource { set; private get; }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (UserDefinePopupEvent != null)
            {
                DataChangedArgs args = new DataChangedArgs
                {
                    Cancel = true
                };
                UserDefinePopupEvent(this, args);               
            }
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (UserDefinePopupEvent != null)
            {
                DataChangedArgs args = new DataChangedArgs
                {
                    Name = mAccountSubjectChangeArgs.Name,
                    NewValue = mTotalAmount,
                    OldValue = RecordUserDefineInputDataValue()
                };
                UserDefinePopupEvent(this, args);
                if (args.Cancel)
                    return;
            }
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close_Click(null,e);
                    break;
            }
        }

        void InitUserDefineInputTemplate()
        {
            if (mAccountSubjectObj == null)
                return;
            
            var lst = DataFactory.Instance.GetTemplateExecuter().GetUdefTemplate("_VoucherEntryUdef");
            if (lst == null)
                return;
            var accountFlagList = AuxiliaryList.Get(Controls.Commons.AuxiliaryType.AccountFlag);
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
                        foreach(var F in accountFlagList)
                        {
                            var mask = 0;
                            if (!int.TryParse(F.name, out mask))
                                continue;
                            if (f == F.no && (mAccountSubjectObj.flag & mask) == 0)
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
                if (DataSource != null)
                {
                    if (DataSource.ContainsKey(item.name))
                        val = DataSource[item.name];
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
            userDefinePanel.DataChangedEvent += UserDefinePanel_DataValueChangeEvent;
        
            SetFocus(userDefinePanel);
        }

        Dictionary<string, object> RecordUserDefineInputDataValue()
        {
            Dictionary<string, object>  map = new Dictionary<string, object>();          
            mUserDefineInputItems.ForEach(udef => {
                if (map.ContainsKey(udef.Name))
                    map[udef.Name] = udef.DataValue;
                else
                    map.Add(udef.Name, udef.DataValue);
            });
            return map;
        }

        private void UserDefinePanel_DataValueChangeEvent(object sender, DataChangedArgs e)
        {
            try
            {
                var uDefInput = sender as UserDefineInput;
                if (uDefInput == null)
                    return;
                                
                var tagLabel = uDefInput.TagLabel;
                if (string.IsNullOrEmpty(tagLabel))
                    return;
                var udefList = userDefinePanel.DataSource;
                var flags = tagLabel.Split('|');
                var f1 = flags[0];
                var f2 = "empty";
                if (flags.Length > 1)
                    f2 = flags[1];

                if (f1 == "price" || f1 == "qty" || f1 == "amount")
                {
                    var udefKey = mAccountSubjectChangeArgs.Name;
                    if (string.IsNullOrEmpty(udefKey))
                    {
                        LogError("GetCurrentUserDefineKey failed!");
                        return;
                    }                   
                    
                    var priceItem = udefList.FirstOrDefault(item => matched(item, f2, "price"));
                    var qtyItem = udefList.FirstOrDefault(item => matched(item, f2, "qty"));
                    var amountItem = udefList.FirstOrDefault(item => matched(item, f2, "amount"));

                    if (priceItem == null || qtyItem == null || amountItem == null)
                    {
                        LogError("get price item or qty item or amount item is null!");
                        return;
                    }
                    if (priceItem.DataValue == null || qtyItem.DataValue == null || amountItem.DataValue == null)
                    {
                        LogDebug("get price or qty is null!");
                        return;
                    }

                    decimal price = 0; decimal qty = 0;decimal amount = 0;
                    if (decimal.TryParse(priceItem.DataValue.ToString(), out price)
                        && decimal.TryParse(qtyItem.DataValue.ToString(), out qty)
                        && decimal.TryParse(amountItem.DataValue.ToString(), out amount))
                    {
                        if (f1 == "amount")
                        {
                            if (qty != 0)
                            {
                                price = decimal.Round(amount / qty,2);
                                var priceInput = userDefinePanel.FindInputByName(priceItem.Name);
                                if (priceInput != null)
                                    priceInput.DataValue = price;
                            }
                            else if (price != 0)
                            {
                                qty = decimal.Round(amount / price,2);
                                var qtyInput = userDefinePanel.FindInputByName(qtyItem.Name);
                                if (qtyInput != null)
                                    qtyInput.DataValue = qty;
                            }
                        }
                        else
                        {
                            amount = decimal.Round(price * qty,2);
                            var amountInput = userDefinePanel.FindInputByName(amountItem.Name);
                            if (amountInput != null)
                                amountInput.DataValue = amount;
                        }
                        
                        CalcTotal(f2, amount);
                    }
                    else
                    {
                        LogError("try parse price or qty or amount failed ! qty:{0},price:{1},amount:{2}.",
                            qtyItem.DataValue, priceItem.DataValue, amountItem.DataValue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                e.Cancel = true;
                FinanceMessageBox.Error(ex.Message);
            }
        }

        public decimal mTotalAmount = 0;
        private void CalcTotal(string f2, decimal amount)
        {
            if (mSumAmount.ContainsKey(f2))
                mSumAmount[f2] = amount;
            else
                mSumAmount.Add(f2, amount);

            mTotalAmount = 0;
            foreach (var kv in mSumAmount)
            {
                mTotalAmount += kv.Value;
            }

            MessageInfo(MessageLevel.INFO, mTotalAmount.ToString("F2", Culinfo));
        }

        

        private void MessageInfo(MessageLevel level, string msg)
        {
            switch (level)
            {
                case MessageLevel.ERR:
                    infoBox.FontSize = 11;
                    infoBox.Foreground = Brushes.Red;
                    break;
                case MessageLevel.WARN:
                    infoBox.FontSize = 11;
                    infoBox.Background = Brushes.Yellow;
                    break;
                default:
                    infoBox.FontSize = 20;
                    infoBox.Foreground = Brushes.Green;
                    break;
            }
            infoBox.Text = msg;
        }

        private bool matched(UserDefineInputItem uDefInput, string f2, string f1)
        {
            var tagLabel = uDefInput.TagLabel;
            if (string.IsNullOrEmpty(tagLabel))
                return false;
            var flags = tagLabel.Split('|');
            if (flags.Contains(f1) && flags.Contains(f2))
                return true;
            return false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitUserDefineInputTemplate();
        }

        void SetFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
            new Action(() =>
            {
                var suc = element.Focus();
                System.Windows.Input.Keyboard.Focus(element);
            }));
        }
    }
}
