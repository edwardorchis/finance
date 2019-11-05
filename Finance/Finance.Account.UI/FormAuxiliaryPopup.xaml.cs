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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Finance.UI.FinanceDelegateEventHandler;

namespace Finance.Account.UI
{
    /// <summary>
    /// FormAccountSubjectPopup.xaml 的交互逻辑
    /// </summary>
    public partial class FormAuxiliaryPopup : Window
    {       
        public AfterSaveEventHandler AfterSaveEvent;
       
        public FormAuxiliaryPopup()
        {
            InitializeComponent();
        }
        public Controls.Commons.AuxiliaryGroup AuxGrp { set; get; }

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
                        ItemSource = new Auxiliary { type =  _originItemSource.type };
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
                            MessageBoxResult ret = FinanceMessageBox.Quest("修改了资料，需要进行保存吗？");
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
            ItemSource.groupId = (int)AuxGrp;
            DataFactory.Instance.GetAuxiliaryExecuter().Save(ItemSource);
            Window_Loaded(this, null);
            AfterSaveEvent?.Invoke();
        }

        bool NeedSave()
        {
            bool result = false;
            result = ItemSource.no != _originItemSource.no
                || ItemSource.name != _originItemSource.name
                || ItemSource.description != _originItemSource.description;
            return result;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public Auxiliary ItemSource
        {
            set
            {
                _itemSource = value;
                xNo = _itemSource.no;
                xName = _itemSource.name;
                xDescription = _itemSource.description;
            }
            private get
            {
                _itemSource.no = xNo;
                _itemSource.name = xName;
                _itemSource.description = xDescription;  
                return _itemSource;
            }
        }

        public string xNo
        {
            get { return (string)GetValue(xNoProperty); }
            set { SetValue(xNoProperty, value); }
        }

        static readonly DependencyProperty xNoProperty =
            DependencyProperty.Register("xNo", typeof(string), typeof(FormAuxiliaryPopup), new PropertyMetadata(""));

        public string xName
        {
            get { return (string)GetValue(xNameProperty); }
            set { SetValue(xNameProperty, value); }
        }

        static readonly DependencyProperty xNameProperty =
            DependencyProperty.Register("xName", typeof(string), typeof(FormAuxiliaryPopup), new PropertyMetadata(""));

        string xDescription
        {
            get { return (string)GetValue(xDescriptionProperty); }
            set { SetValue(xDescriptionProperty, value); }
        }
        static readonly DependencyProperty xDescriptionProperty =
            DependencyProperty.Register("xDescription", typeof(string), typeof(FormAuxiliaryPopup), new PropertyMetadata(""));

        Auxiliary _originItemSource = null;
        Auxiliary _itemSource = new Auxiliary();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _originItemSource = JsonConvert.DeserializeObject<Auxiliary>(JsonConvert.SerializeObject(_itemSource));
        }

    }

    


}
