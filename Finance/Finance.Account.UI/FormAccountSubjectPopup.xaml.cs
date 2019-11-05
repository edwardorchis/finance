using Finance.Account.Controls;
using Finance.Account.Controls.Commons;
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
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormAccountSubjectPopup.xaml 的交互逻辑
    /// </summary>
    public partial class FormAccountSubjectPopup : Window
    {
        public AfterSaveEventHandler AfterSaveEvent;

        public FormAccountSubjectPopup()
        {
            InitializeComponent();

            cmbGroup.ItemsSource = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.AccountGroup);

            cmbDirection.ItemsSource = new List<Auxiliary>() {
                new Auxiliary{id = 1,name ="借方"},new Auxiliary{id =-1,name ="贷方"}
            };
                        
            var lst = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.Invalid);
            if (lst == null)
                lst = new List<Auxiliary>();
            lst.Insert(0, new Auxiliary() { no = "0", groupId = (int)SDK.AuxiliaryGroup.AccountItems });
            cmbActItemGrp.ItemsSource = lst.FindAll(a => a.groupId == (int)SDK.AuxiliaryGroup.AccountItems);
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
                        ItemSource = new AccountSubject { direction =1};
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
                        Close();
                        break;
                    case "close":
                    case "exit":
                        if (NeedSave())
                        {
                            MessageBoxResult ret = FinanceMessageBox.Quest("修改了科目，需要进行保存吗？");
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
            Console.WriteLine(JsonConvert.SerializeObject(ItemSource));
            DataFactory.Instance.GetAccountSubjectExecuter().Save(ItemSource);
            Window_Loaded(this, null);
            AfterSaveEvent?.Invoke();
        }

        bool NeedSave()
        {
            bool result = false;
            result = ItemSource.no != _originItemSource.no
                || ItemSource.name != _originItemSource.name
                || ItemSource.direction != _originItemSource.direction
                || ItemSource.groupId != _originItemSource.groupId
                || ItemSource.flag != _originItemSource.flag
                || ItemSource.actItemGrp != _originItemSource.actItemGrp
                || ItemSource.actUint != _originItemSource.actUint;
            return result;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        string xNo
        {
            get => (string)GetValue(xNoProperty);
            set => SetValue(xNoProperty, value);
        }

        static readonly DependencyProperty xNoProperty =
            DependencyProperty.Register("xNo", typeof(string), typeof(FormAccountSubjectPopup), new PropertyMetadata(""));

        string xName
        {
            get { return (string)GetValue(xNameProperty); }
            set { SetValue(xNameProperty, value); }
        }

        static readonly DependencyProperty xNameProperty =
            DependencyProperty.Register("xName", typeof(string), typeof(FormAccountSubjectPopup), new PropertyMetadata(""));

        long xDirection
        {
            get { return (long)GetValue(xDirectionProperty); }
            set { SetValue(xDirectionProperty, value); }
        }

        static readonly DependencyProperty xDirectionProperty =
            DependencyProperty.Register("xDirection", typeof(long), typeof(FormAccountSubjectPopup), new PropertyMetadata((long)1));

        long xGroupId
        {
            get { return (long)GetValue(xGroupIdProperty); }
            set { SetValue(xGroupIdProperty, value); }
        }

        static readonly DependencyProperty xGroupIdProperty =
            DependencyProperty.Register("xGroupId", typeof(long), typeof(FormAccountSubjectPopup), new PropertyMetadata((long)0));

        string xActItemGrp
        {
            get { return (string)GetValue(xActItemGrpProperty); }
            set { SetValue(xActItemGrpProperty, value); }
        }

        static readonly DependencyProperty xActItemGrpProperty =
            DependencyProperty.Register("xActItemGrp", typeof(string), typeof(FormAccountSubjectPopup), new PropertyMetadata(""));

        string xActUint
        {
            get { return (string)GetValue(xActUintProperty); }
            set { SetValue(xActUintProperty, value); }
        }

        static readonly DependencyProperty xActUintProperty =
            DependencyProperty.Register("xActUint", typeof(string), typeof(FormAccountSubjectPopup), new PropertyMetadata(""));


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _originItemSource = JsonConvert.DeserializeObject<AccountSubject>(JsonConvert.SerializeObject(_itemSource));
            FillUdefPanel();
        }

        AccountSubject _itemSource = new AccountSubject { direction = 1 };
        AccountSubject _originItemSource = null;

        public AccountSubject ItemSource {
            set {
                _itemSource = value;
                _itemSource.no = _itemSource.no == null? "" : _itemSource.no.TrimStart();
                xNo = _itemSource.no;
                xName = _itemSource.name;
                xDirection = _itemSource.direction;
                xGroupId = _itemSource.groupId;
                xActItemGrp = _itemSource.actItemGrp;
                xActUint = _itemSource.actUint;
            }
            private get {
                _itemSource.no = xNo;
                _itemSource.name = xName;
                _itemSource.direction =(int)xDirection;
                _itemSource.groupId = xGroupId;
                _itemSource.flag = ReadUdefPanel();
                _itemSource.actItemGrp = xActItemGrp;
                _itemSource.actUint = xActUint;
                return _itemSource;
            }
        }

        private void FillUdefPanel()
        {
            var items = FlagCheckBoxItems.FindAll(f => (_itemSource.flag & f.value) != 0);
            if (items == null)
                return;
            foreach(var item in items)
                item.isSelected = true;
            chkListBox.ItemsSource = FlagCheckBoxItems;
        }

        int ReadUdefPanel()
        {
            var iRet = 0;
            _FlagCheckBoxItems = (List<FlagCheckBoxItem>)chkListBox.ItemsSource;
            foreach (var item in FlagCheckBoxItems)
            {
                if (!item.isSelected)
                    continue;
                iRet = iRet | item.value;
            }

            int actItemGrp = 0;
            int.TryParse(xActItemGrp, out actItemGrp);
            if (actItemGrp > 0)
                iRet = iRet | 1;
            if (!string.IsNullOrWhiteSpace(xActUint))
                iRet = iRet | 2;
            return iRet;
        }
       

        List<FlagCheckBoxItem> _FlagCheckBoxItems = null;
        List<FlagCheckBoxItem> FlagCheckBoxItems
        {
            get
            {
                if (_FlagCheckBoxItems != null)
                    return _FlagCheckBoxItems;
                List<Auxiliary> flagList = DataFactory.Instance.GetAuxiliaryExecuter().List(SDK.AuxiliaryType.AccountFlag);
                if (flagList == null)
                    flagList = new List<Auxiliary>();
                flagList.ForEach(f => {
                    var item = new FlagCheckBoxItem();
                    item.display = f.description;
                    var val = 0;
                    int.TryParse(f.name, out val);
                    item.value = val;
                    item.isSelected = false;
                    if (_FlagCheckBoxItems == null)
                        _FlagCheckBoxItems = new List<FlagCheckBoxItem>();
                    _FlagCheckBoxItems.Add(item);
                });
                return _FlagCheckBoxItems;
            }
        }

        public class FlagCheckBoxItem
        {
            public string display { set; get; }
            public int value { set; get; }
            public bool isSelected { set; get; }
        }
    }

    


}
