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
    public partial class FormUdefTemplatePopup : Window
    {
        public AfterSaveEventHandler AfterSaveEvent;
        UdefTemplateItem _itemSource = new UdefTemplateItem();
        UdefTemplateItem _originItemSource = null;

        public FormUdefTemplatePopup()
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
                        _itemSource = new UdefTemplateItem();
                        _originItemSource = new UdefTemplateItem();
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
            DataFactory.Instance.GetTemplateExecuter().SaveUdefTemplate(_itemSource);
            AfterSaveEvent?.Invoke();
        }

        bool NeedSave()
        {
            bool result = false;
            result = StringNotEqNullAndWhiteSpace( _originItemSource.tableName, ItemSource.tableName)
                || StringNotEqNullAndWhiteSpace(_originItemSource.name, ItemSource.name)
                || StringNotEqNullAndWhiteSpace(_originItemSource.label, ItemSource.label)
                || StringNotEqNullAndWhiteSpace(_originItemSource.dataType, ItemSource.dataType)
                ||  _originItemSource.tabIndex != ItemSource.tabIndex
                || StringNotEqNullAndWhiteSpace(_originItemSource.defaultVal, ItemSource.defaultVal)
                || StringNotEqNullAndWhiteSpace(_originItemSource.reserved, ItemSource.reserved)
                || StringNotEqNullAndWhiteSpace(_originItemSource.tagLabel, ItemSource.tagLabel)
                || _originItemSource.width != ItemSource.width;
            return result;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public UdefTemplateItem ItemSource
        {
            set {
                _itemSource = value;
                tableName = _itemSource.tableName;
                name = _itemSource.name;
                label = _itemSource.label;
                dataType = _itemSource.dataType;
                tabIndex = _itemSource.tabIndex;
                defaultVal = _itemSource.defaultVal;
                reserved = _itemSource.reserved;
                tagLabel = _itemSource.tagLabel;
                xwidth = _itemSource.width;
            }
            get {
                _itemSource.tableName = tableName;
                _itemSource.name = name;
                _itemSource.label = label;
                _itemSource.dataType = dataType;
                _itemSource.tabIndex = tabIndex;
                _itemSource.defaultVal = defaultVal;
                _itemSource.reserved = reserved;
                _itemSource.tagLabel = tagLabel;
                _itemSource.width = xwidth;
                return _itemSource;
            }
        }

        public string tableName
        {
            get { return (string)GetValue(tableNameProperty); }
            set { SetValue(tableNameProperty, value); }
        }

        static readonly DependencyProperty tableNameProperty =
            DependencyProperty.Register("tableName", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));


        public string name
        {
            get { return (string)GetValue(xNameProperty); }
            set { SetValue(xNameProperty, value); }
        }

        static readonly DependencyProperty xNameProperty =
            DependencyProperty.Register("name", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));

        public string label
        {
            get { return (string)GetValue(labelProperty); }
            set { SetValue(labelProperty, value); }
        }

        static readonly DependencyProperty labelProperty =
            DependencyProperty.Register("label", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));

        public string dataType
        {
            get { return (string)GetValue(dataTypeFormProperty); }
            set { SetValue(dataTypeFormProperty, value); }
        }

        static readonly DependencyProperty dataTypeFormProperty =
            DependencyProperty.Register("dataType", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));
 
        public string defaultVal
        {
            get { return (string)GetValue(defaultValProperty); }
            set { SetValue(defaultValProperty, value); }
        }

        static readonly DependencyProperty defaultValProperty =
            DependencyProperty.Register("defaultVal", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));
        public string reserved
        {
            get { return (string)GetValue(reservedProperty); }
            set { SetValue(reservedProperty, value); }
        }

        static readonly DependencyProperty reservedProperty =
            DependencyProperty.Register("reserved", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));
        public string tagLabel
        {
            get { return (string)GetValue(tagLabelProperty); }
            set { SetValue(tagLabelProperty, value); }
        }

        static readonly DependencyProperty tagLabelProperty =
            DependencyProperty.Register("tagLabel", typeof(string), typeof(FormUdefTemplatePopup), new PropertyMetadata(""));
        public int tabIndex
        {
            get { return (int)GetValue(tabIndexProperty); }
            set { SetValue(tabIndexProperty, value); }
        }

        static readonly DependencyProperty tabIndexProperty =
            DependencyProperty.Register("tabIndex", typeof(int), typeof(FormUdefTemplatePopup), new PropertyMetadata(0));


        public int xwidth
        {
            get { return (int)GetValue(xwidthProperty); }
            set { SetValue(xwidthProperty, value); }
        }

        static readonly DependencyProperty xwidthProperty =
            DependencyProperty.Register("xwidth", typeof(int), typeof(FormUdefTemplatePopup), new PropertyMetadata(0));


        


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _originItemSource = JsonConvert.DeserializeObject<UdefTemplateItem>(JsonConvert.SerializeObject(_itemSource));
                     
        }

    }

    


}
