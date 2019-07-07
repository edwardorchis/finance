using Finance.Account.Controls;
using Finance.Account.Data;
using Finance.Account.SDK;
using Finance.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using static Finance.Utils.CommonUtils;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormAccountSubjectPopup.xaml 的交互逻辑
    /// </summary>
    public partial class FormMenuEditPopup : Window
    {
        public AfterSaveEventHandler AfterSaveEvent;
        MenuTableMap _itemSource = new MenuTableMap();
        MenuTableMap _originItemSource = null;

        public FormMenuEditPopup()
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
                        _itemSource = new MenuTableMap();
                        _originItemSource = new MenuTableMap();
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
                            MessageBoxResult ret = FinanceMessageBox.Quest("修改了项目，需要进行保存吗？");
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
            DataFactory.Instance.GetSystemProfileExecuter().SaveMenuTable(_itemSource);
            AfterSaveEvent?.Invoke();
        }

        bool NeedSave()
        {
            bool result = false;
            result = StringNotEqNullAndWhiteSpace(_originItemSource.group, ItemSource.group)
                || StringNotEqNullAndWhiteSpace(_originItemSource.name, ItemSource.name)
                || StringNotEqNullAndWhiteSpace(_originItemSource.financeForm, ItemSource.financeForm) 
                || StringNotEqNullAndWhiteSpace(_originItemSource.header, ItemSource.header)
                || _originItemSource.index != ItemSource.index;
            return result;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        public MenuTableMap ItemSource
        {
            set
            {
                _itemSource = value;
                group = value.group;
                name = value.name;
                financeForm = value.financeForm;
                header = value.header;
                index = value.index;
            }
            private get
            {
                _itemSource.group = group;
                _itemSource.name = name;
                _itemSource.financeForm = financeForm;
                _itemSource.header = header;
                _itemSource.index = index;
                return _itemSource;
            }
        }


        string group
        {
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("group", typeof(string), typeof(FormMenuEditPopup), new PropertyMetadata(""));


        string name
        {
            get { return (string)GetValue(xNameProperty); }
            set { SetValue(xNameProperty, value); }
        }

        static readonly DependencyProperty xNameProperty =
            DependencyProperty.Register("name", typeof(string), typeof(FormMenuEditPopup), new PropertyMetadata(""));

        string header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("header", typeof(string), typeof(FormMenuEditPopup), new PropertyMetadata(""));

        string financeForm
        {
            get { return (string)GetValue(FinanceFormProperty); }
            set { SetValue(FinanceFormProperty, value); }
        }

        static readonly DependencyProperty FinanceFormProperty =
            DependencyProperty.Register("financeForm", typeof(string), typeof(FormMenuEditPopup), new PropertyMetadata(""));


        int index
        {
            get { return (int)GetValue(xIndexProperty); }
            set { SetValue(xIndexProperty, value); }
        }

        static readonly DependencyProperty xIndexProperty =
            DependencyProperty.Register("index", typeof(int), typeof(FormMenuEditPopup), new PropertyMetadata(0));


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _originItemSource = JsonConvert.DeserializeObject<MenuTableMap>(JsonConvert.SerializeObject(_itemSource));

           
            txtGroup.ItemsSource = UiUtils.FirstMenuDisplayNameMap;

            List<string> lstFinanceForm = new List<string>();
            Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(t=> {
                if (t.IsSubclassOf(typeof(FinanceForm)))
                {
                    lstFinanceForm.Add(t.Name);
                }
            });
            
            txtFinanceForm.ItemsSource = lstFinanceForm;
        }

    }

    


}
