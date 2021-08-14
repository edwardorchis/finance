using Finance.Account.Controls.Commons;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Finance.Account.Controls.AmountInputBox;
using static Finance.Account.Controls.Commons.Consts;

namespace Finance.Account.Controls
{
    /// <summary>
    /// VoucherGrid.xaml 的交互逻辑
    /// </summary>
    public partial class VoucherGrid : UserControl
    {
        static int MAX_ROWS = 100;
        static int MAX_COLUMNS = 5;

        string[,] m_Controls = new string[MAX_ROWS, MAX_COLUMNS];

        string m_LastFocusControl = "";        


        public event RowChangeEventHandler RowChangeEvent;        
        public event DataChangedEventHandler AccountSubjectChangeEvent;
        public event MessageEventHandler MessageEvent;
        public event CellKeyDownEventHandler CellKeyDownEvent;
        public event BeginLayoutEventHandler BeginLayoutEvent;
        public event EndLayoutEventHandler EndLayoutEvent;
        public event DisplayHookEventHandler DisplayHookEvent;

        protected void OnRowChangeEvent(Control sender,string oldKey,string newKey) {
            if (RowChangeEvent == null)
                return;
            RowChangeEventArgs args = new RowChangeEventArgs();
            args.OldKey = oldKey;
            args.NewKey = newKey;
            LogDebug(string.Format("OnRowChangeEvent:oldKey[{0}]newKey[{1}]",oldKey,newKey));
            RowChangeEvent(grid, args);
            if (args.Cancel)
            {
                SetLastFocus();
                throw new FinanceAccountControlException(FinanceAccountControlErrorCode.CANCEL);
            }
        }

        public VoucherGrid()
        {            
            InitializeComponent();           
        }

        List<VoucherGridItem> m_DataSource = new List<VoucherGridItem>();
        public List<VoucherGridItem> DataSource {
            get
            {
                LogDebug("enter DataSource");
                HoldDataSource();
                return m_DataSource;
            }
            set
            {
                CurrentRowIndex = -1;
                m_DataSource = value;
            }
        }

        public int CurrentRowIndex { set; get; } = -1;

        bool isReadOnly = false;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set {
                isReadOnly = value;
                for (int i = 0; i < MAX_ROWS; i++)
                {
                    for (int j = 0; j < MAX_COLUMNS; j++)
                    {
                        if (j > 0 && j < 5)
                        {
                            var ctlName = m_Controls[i, j];
                            if (ctlName == null || string.IsNullOrEmpty(ctlName))
                                return;
                            var ctl = grid.FindName(ctlName);
                            if (ctl == null)
                                return;
                            if (j == 1)
                            {
                                (ctl as AuxiliaryBox).IsReadOnly = isReadOnly;
                            }
                            else if (j == 2)
                            {
                                (ctl as AccountSubjectBox).IsReadOnly = isReadOnly;
                            }
                            else if (j == 3 || j == 4)
                            {
                                (ctl as AmountInputBox).IsReadOnly = IsReadOnly;
                            }
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            LogDebug("enter Clear");
            m_DataSource.Clear();
            CurrentRowIndex = -1;
            Refresh();
            OnRowChangeEvent(null, "", "");
        }

        public void AppendRow()
        {
            LogDebug("enter AppendRow");
            HoldDataSource();
            if (CurrentRowIndex >= 0)
            {
                var ele = CheckRow(CurrentRowIndex);
                if (ele != null)
                {
                    SetFocus(ele);
                    return;
                }
            }
            var m = new VoucherGridItem();
            m_DataSource.Add(m);
            var index = m_DataSource.Count - 1;
            if (index >= 0)
            {
                m_LastFocusControl = "txtContent_" + index;
            }
            Refresh();
        }

        public void InsertRow(int index)
        {
            LogDebug("enter InsertRow");
            if (index >= 0)
            {
                HoldDataSource();
                var oldKey = "";
                if (m_DataSource.Count > 1 && index < m_DataSource.Count)
                    oldKey = m_DataSource[index].UniqueKey;
                var m = new VoucherGridItem();
                m_DataSource.Insert(index, m);                
                Refresh();               
                OnRowChangeEvent(null, oldKey, m.UniqueKey);
            }
        }

        public void DeleteRow(int index)
        {
            LogDebug("enter DeleteRow");
            if (index >=0)
            {
                HoldDataSource();               
                m_DataSource.RemoveAt(index);
                CurrentRowIndex = index - 1;
                if (index == 0)
                    CurrentRowIndex = index;
                m_LastFocusControl = "";
                Refresh();
                var newKey = "";
                if (CurrentRowIndex >= 0 && CurrentRowIndex < m_DataSource.Count) 
                    newKey = m_DataSource[CurrentRowIndex].UniqueKey;
                OnRowChangeEvent(null, "", newKey);
            }
        }

        /// <summary>
        /// 生效，把数据刷到界面上
        /// </summary>
        public void Refresh()
        {            
            LogDebug(string.Format("Refresh[{0}]",m_DataSource.Count));
            ReleaseControls();
            if(grid.RowDefinitions.Count>0)
                grid.RowDefinitions.RemoveRange(0, grid.RowDefinitions.Count);
            int index = 0;
            m_DataSource.ForEach(item =>
            {
                var row = new RowDefinition();
                row.Height = new GridLength(60);
                grid.RowDefinitions.Add(row);

                Label lblIndex = new Label();
                lblIndex.Name = "lblIndex_" + index;
                lblIndex.Background = Consts.HIGHLIGHT_BRUSH;
                lblIndex.VerticalContentAlignment = VerticalAlignment.Center;
                lblIndex.BorderBrush = Consts.BLACK_BRUSH;
                lblIndex.BorderThickness = new Thickness(1, 0, 1, 1);
                lblIndex.Content = index + 1;
                lblIndex.GotFocus += Row_GotFocus;
                lblIndex.Tag = item.UniqueKey;

                grid.RegisterName("lblIndex_" + index, lblIndex);
                grid.Children.Add(lblIndex);
                m_Controls[index, 0] = "lblIndex_" + index;
                lblIndex.SetValue(Grid.RowProperty, index);
                lblIndex.SetValue(Grid.ColumnProperty, 0);

                AuxiliaryBox txtContent = new AuxiliaryBox();
                txtContent.Name = "txtContent_" + index;
                txtContent.BorderBrush = Consts.BLACK_BRUSH;
                txtContent.BorderThickness = new Thickness(0, 0, 1, 1);
                txtContent.TextWrapping = TextWrapping.Wrap;
                txtContent.Text = item.Content;                              
                txtContent.GotFocus += Row_GotFocus;
                txtContent.TabIndex = this.TabIndex + index * 4 + 0;

                grid.RegisterName("txtContent_" + index, txtContent);
                grid.Children.Add(txtContent);
                m_Controls[index, 1] = "txtContent_" + index;
                Grid.SetRow(txtContent, index);
                Grid.SetColumn(txtContent, 1);

                AccountSubjectBox txtAccountSubject = new AccountSubjectBox();
                txtAccountSubject.DisplayHookEvent += (sender, e) => {
                    e.Key = item.UniqueKey;
                    DisplayHookEvent?.Invoke(sender, e);
                };
                txtAccountSubject.Name = "txtAccountSubject_" + index;
                txtAccountSubject.BorderBrush = Consts.BLACK_BRUSH;
                txtAccountSubject.BorderThickness = new Thickness(0, 0, 1, 1);                
                txtAccountSubject.Id=item.AccountSubjectId;
                txtAccountSubject.IsReadOnly = this.IsReadOnly;
                txtAccountSubject.GotFocus += Row_GotFocus;
                txtAccountSubject.DataChangedEvent += (sender, e)=> {
                    if (CurrentRowIndex < 0 || CurrentRowIndex >= m_DataSource.Count)
                        return;
                    e.Name = m_DataSource[CurrentRowIndex].UniqueKey;
                    AccountSubjectChangeEvent?.Invoke(sender, e);
                    var actSubjectObj = txtAccountSubject.Value;
                    if ((actSubjectObj.flag & (int)AccountFlag.FLAG_USDEF_MASK) != 0)
                        AdornerHelper.SetHasAdorner(txtAccountSubject, 1);
                    else
                        AdornerHelper.SetHasAdorner(txtAccountSubject, 0);
                };
                txtAccountSubject.KeyDown += (sender, e) => {
                    if (IsReadOnly)
                        return;
                    CellKeyDownEvent?.Invoke(sender, e);
                };
               
                txtAccountSubject.TabIndex = this.TabIndex + index * 4 + 1;
                grid.RegisterName("txtAccountSubject_" + index, txtAccountSubject);
                grid.Children.Add(txtAccountSubject);
                m_Controls[index, 2] = "txtAccountSubject_" + index;
                Grid.SetRow(txtAccountSubject, index);
                Grid.SetColumn(txtAccountSubject, 2);
                if ((item.AccountSubject.flag & (int)AccountFlag.FLAG_USDEF_MASK) != 0)
                    AdornerHelper.SetHasAdorner(txtAccountSubject, 1);               

                AmountInputBox aInputDebits = new AmountInputBox();
                aInputDebits.Name = "aInputDebits_" + index;
                aInputDebits.BorderBrush = Consts.BLACK_BRUSH;
                aInputDebits.BorderThickness = new Thickness(0, 0, 1, 1);
                aInputDebits.Value = item.DebitsAmount;
                aInputDebits.IsEnabled = !this.IsReadOnly;
                aInputDebits.GotFocus += Row_GotFocus;
                aInputDebits.LostFocus += AInputDebits_LostFocus;
                aInputDebits.KeyDown += AInputDebits_KeyDown;
                aInputDebits.TabIndex = this.TabIndex + index * 4 + 2;
                aInputDebits.BeforeInputEvent += AmountInput_BeforeInput;
                grid.RegisterName("aInputDebits_" + index, aInputDebits);
                grid.Children.Add(aInputDebits);
                m_Controls[index, 3] = "aInputDebits_" + index;
                Grid.SetRow(aInputDebits, index);
                Grid.SetColumn(aInputDebits, 3);

                AmountInputBox aInputCredit = new AmountInputBox();
                aInputCredit.Name = "aInputCredit_" + index;
                aInputCredit.BorderBrush = Consts.BLACK_BRUSH;
                aInputCredit.BorderThickness = new Thickness(0, 0, 1, 1);
                aInputCredit.Value = item.CreditAmount;
                aInputCredit.IsEnabled = !this.IsReadOnly;
                aInputCredit.GotFocus += Row_GotFocus;
                aInputCredit.LostFocus += AInputCredit_LostFocus;
                aInputCredit.KeyDown += AInputDebits_KeyDown;
                aInputCredit.TabIndex = this.TabIndex + index * 4 + 3;
                aInputCredit.BeforeInputEvent += AmountInput_BeforeInput;
                grid.RegisterName("aInputCredit_" + index, aInputCredit);
                grid.Children.Add(aInputCredit);
                m_Controls[index, 4] = "aInputCredit_" + index;
                Grid.SetRow(aInputCredit, index);
                Grid.SetColumn(aInputCredit, 4);

                index++;
            });
            //if (index > 0)
            //    CurrentRowIndex = 0;
            CalcTotal();
            SetLastFocus();
        }

        public bool SetLastFocus()
        {          
            if (!string.IsNullOrEmpty(m_LastFocusControl))
            {
                var ctl = grid.FindName(m_LastFocusControl);
                if (ctl != null)
                {
                    var ele1 = ctl as FrameworkElement;
                    if (ele1 != null)
                    {
                        if (!this.Focusable)
                        {
                            var sn = m_LastFocusControl.Substring(m_LastFocusControl.IndexOf("_") + 1);
                            int OldRowIndex = CurrentRowIndex;
                            CurrentRowIndex = int.Parse(sn);
                            return false;
                        }
                        SetFocus(ele1);
                        return true;
                    }
                }
            }
            return false;
        }

        private void AInputDebits_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsReadOnly)
                return;
            if (e.Key == Key.OemPlus)
            {
                var name = (sender as FrameworkElement).Name;
                var index = int.Parse(name.Substring(name.IndexOf("_") + 1));
                if ((grid.FindName("aInputCredit_" + index) as AmountInputBox).Value != 0
                    || (grid.FindName("aInputDebits_" + index) as AmountInputBox).Value != 0)
                    return;

                var dTotalCredit = totalCredit.Value;
                var dTotalDebits = totalDebits.Value;
                
                if (name.StartsWith("aInputDebits"))
                {
                    dTotalDebits -= (sender as AmountInputBox).Value;
                }
                else
                {
                    dTotalCredit -= (sender as AmountInputBox).Value;
                }
                
                var diff = dTotalDebits - dTotalCredit;
                if (diff > 0)
                {
                    (grid.FindName("aInputCredit_" + index) as AmountInputBox).Value = diff;
                }
                else
                {
                    (grid.FindName("aInputDebits_" + index) as AmountInputBox).Value = Math.Abs(diff);
                }
                CalcTotal();
            }
        }

        private void AInputCredit_LostFocus(object sender, RoutedEventArgs e)
        {           
            if (IsReadOnly)
                return;
            //LogDebug("enter AInputCredit_LostFocus");
            //CalcTotal();
            var currentIndex = int.Parse((sender as AmountInputBox).Name.Replace("aInputCredit_", ""));
            LogDebug(string.Format("enter AInputCredit_LostFocus [{0}]",currentIndex));
            if (currentIndex == grid.RowDefinitions.Count - 1)
            {
                AppendRow();
            }
        }

        private void AInputDebits_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            LogDebug("enter AInputDebits_LostFocus");
            CalcTotal();
        }

        void CalcTotal()
        {
            LogDebug("CalcTotal");
            HoldDataSource();
            int i = 0;
            decimal dTotalCredit = 0M;
            while (i < MAX_ROWS)
            {
                var name = "aInputCredit_" + i;
                var ctl = grid.FindName(name);
                if (ctl == null)
                    break;
                var input = ctl as AmountInputBox;
                dTotalCredit += input.Value;
                i++;
            }
            totalCredit.Value = dTotalCredit;

            i = 0;
            decimal dTotalDebits = 0M;
            while (i < MAX_ROWS)
            {
                var name = "aInputDebits_" + i;
                var ctl = grid.FindName(name);
                if (ctl == null)
                    break;
                var input = ctl as AmountInputBox;
                dTotalDebits += input.Value;
                i++;
            }
            totalDebits.Value = dTotalDebits;
            CheckRow(CurrentRowIndex);
        }


        void SetFocus(FrameworkElement element)
        {
            LogDebug("BeginLayoutEvent");
            BeginLayoutEvent?.Invoke();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
            new Action(() =>
            {
                var suc = element.Focus();
                System.Windows.Input.Keyboard.Focus(element);
                EndLayoutEvent?.Invoke();
                LogDebug("EndLayoutEvent");
            }));
        }

        FrameworkElement CheckRow(int rowIndex)
        {
            try
            {
                if (rowIndex < 0)
                    return null;
                if (IsEmptyRow(rowIndex) && (rowIndex == m_DataSource.Count - 1)  && (rowIndex -1 >= 0))
                {
                    var ele = CheckRow(rowIndex - 1);
                    return ele;
                }          
                if (rowIndex >= m_DataSource.Count)
                    return null;
                var row = m_DataSource[rowIndex];
                if (row.AccountSubjectId == 0L && !isReadOnly)
                {
                    MessageEvent?.Invoke(MessageLevel.ERR, "科目不能为空！");
                    var ele1 = grid.FindName("txtAccountSubject_" + rowIndex) as FrameworkElement;                    
                    return ele1;
                }
                else if (row.CreditAmount == 0M && row.DebitsAmount == 0M && !isReadOnly)
                {
                    MessageEvent?.Invoke(MessageLevel.ERR, "金额不能为零！");
                    var ele1 = grid.FindName("aInputDebits_" + rowIndex) as FrameworkElement;
                    return ele1;
                }
                var dTotalCredit = totalCredit.Value;
                var dTotalDebits = totalDebits.Value;
                if (dTotalCredit != dTotalDebits)
                {
                    MessageEvent?.Invoke(MessageLevel.WARN, "借贷不平衡！");
                    return null;
                }
                MessageEvent?.Invoke(MessageLevel.INFO, "");
            }
            catch (Exception ex)
            {
                FinanceMessageBox.Error(ex.Message);
            }
            return null;
        }

        private void AmountInput_BeforeInput(object sender, BeforeInputEventArgs e)
        {
            AmountInputBox input = sender as AmountInputBox;
            var rowIndex = int.Parse(input.Name.Substring(m_LastFocusControl.IndexOf("_") + 1));
            var aInputCreditAmount = GetAmountInputBoxByName("aInputCredit_" + rowIndex).Value;
            var aInputDebitsAmount = GetAmountInputBoxByName("aInputDebits_" + rowIndex).Value;
            if (input.Name.StartsWith("aInputCredit_") && aInputDebitsAmount > 0)
                e.Disable = true;
            else if (input.Name.StartsWith("aInputDebits_") && aInputCreditAmount > 0)
                e.Disable = true;
        }        

        AmountInputBox GetAmountInputBoxByName(string name)
        {            
            var ctl = grid.FindName(name);
            if (ctl == null)
                return null;
            var input = ctl as AmountInputBox;
            return input;
        }

        private void Row_GotFocus(object sender, RoutedEventArgs e)
        {            
            var ctl = sender as Control;
            var name = ctl.Name;            
            if (!string.IsNullOrEmpty(name))
            {
                var sn = name.Substring(name.IndexOf("_") + 1);
                int OldRowIndex = CurrentRowIndex;
                CurrentRowIndex = int.Parse(sn);

                if (OldRowIndex != CurrentRowIndex)
                {
                    LogDebug(string.Format("enter Row_GotFocus [{0}]", CurrentRowIndex));
                    HoldDataSource();
                    var ele = CheckRow(OldRowIndex);
                    if (ele == null)
                    {
                        var oldKey = "";
                        if (OldRowIndex >= 0 && OldRowIndex < m_DataSource.Count)
                            oldKey = m_DataSource[OldRowIndex].UniqueKey;
                        var newKey = "";
                        if (CurrentRowIndex < m_DataSource.Count)
                            newKey = m_DataSource[CurrentRowIndex].UniqueKey;
                        OnRowChangeEvent(ctl, oldKey, newKey);                        
                    }
                    else
                    {
                        SetFocus(ele);
                        return;
                    }                    
                }
                m_LastFocusControl = name;               
            }
        }

        bool IsEmptyRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex > m_DataSource.Count - 1)
                return true ;
            var row = m_DataSource[rowIndex];
            if (string.IsNullOrEmpty(row.Content) && row.AccountSubjectId == 0 && row.CreditAmount == 0 && row.DebitsAmount == 0)
                return true;
            return false;
        }
        /// <summary>
        /// 释放所有的编辑控件
        /// </summary>
        void ReleaseControls()
        {
            for (int i = 0; i < MAX_ROWS; i++)
            {
                for (int j = 0; j < MAX_COLUMNS; j++)
                {
                    string ctl = m_Controls[i, j];
                    if (ctl != null && !string.IsNullOrEmpty(ctl))
                    {
                        Control c = (Control)(grid.FindName(ctl));
                        grid.Children.Remove(c);
                        grid.UnregisterName(ctl);
                        m_Controls[i, j] = null;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            
        }
        /// <summary>
        /// 暂存数据
        /// </summary>
        void HoldDataSource()
        {
            LogDebug(string.Format("enter HoldDataSource [{0}]", grid.RowDefinitions.Count));
            List<VoucherGridItem> tmpDataSource = new List<VoucherGridItem>();           
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                VoucherGridItem item = new VoucherGridItem();
                for (int j = 0; j < MAX_COLUMNS; j++)
                {
                    string ctl = m_Controls[i, j];
                    if (!string.IsNullOrEmpty(ctl))
                    {
                        if (j == 0)
                        {
                            item.UniqueKey = ((Label)(grid.FindName(ctl))).Tag.ToString();
                        }
                        else if (j == 1)
                        {
                            item.Content = ((AuxiliaryBox)(grid.FindName(ctl))).Text;
                        }
                        else if (j == 2)
                        {
                            item.AccountSubjectId = ((AccountSubjectBox)(grid.FindName(ctl))).Id;
                        }
                        else if (j == 3)
                        {
                            item.DebitsAmount = ((AmountInputBox)(grid.FindName(ctl))).Value;
                        }
                        else if (j == 4)
                        {
                            item.CreditAmount = ((AmountInputBox)(grid.FindName(ctl))).Value;
                        }
                        
                    }
                    else
                    {
                        LogDebug(string.Format("get row control failed [{0},{1}]",i,j));
                        goto next_row;
                    }
                }
                tmpDataSource.Add(item);
            next_row:;
            }
            if(tmpDataSource.Count != 0)
                m_DataSource = tmpDataSource;
            LogDebug(string.Format("exit HoldDataSource[{0}][{1}]", tmpDataSource.Count, m_DataSource.Count));
        }
        private bool isShow = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isShow)
            {
                SetLastFocus();
                return;
            }                
            Load();
            isShow = true;
        }

        public void Load()
        {
            LogDebug("Load");
            DataSource = new List<VoucherGridItem>();
            DataSource.Add(new VoucherGridItem());
            Refresh();
        }

        

    }
}
