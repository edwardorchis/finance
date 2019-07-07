using Finance.Account.Controls.Commons;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Finance.Account.Controls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    internal partial class AccountSubjectPopup : Window
    {
        public delegate void SelectedEventHandler(SelectedEventArgs e);
        public class SelectedEventArgs : EventArgs
        {
            public AccountSubjectObj subjectObj { set; get; }
            public bool Handled { set; get; }
        }
        public SelectedEventHandler SelectedEvent;

        public AccountSubjectPopup()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = AccountSubjectList.Get();
            list.RemoveAll(aso => aso.isDeleted);
            Dictionary<long, AccountClass> keyValuePairs = new Dictionary<long, AccountClass>();
            var listAccountGroup = AuxiliaryList.Get(AuxiliaryType.AccountGroup);
            foreach (var group in listAccountGroup)
            {
                keyValuePairs.Add(group.id, (AccountClass)(group.parentId));
            }
            list.ForEach(a => { a.accountClass = keyValuePairs[a.groupId]; });

            this.treeView1.ItemsSource = GetNodeList(list, AccountClass.Asset);
            //ExpandTree(treeView1); 

            this.treeView2.ItemsSource = GetNodeList(list, AccountClass.Liability);
            //ExpandTree(treeView2);

            this.treeView3.ItemsSource = GetNodeList(list, AccountClass.Common);
            //ExpandTree(treeView3);

            this.treeView4.ItemsSource = GetNodeList(list, AccountClass.Equity);
            //ExpandTree(treeView4);

            this.treeView5.ItemsSource = GetNodeList(list, AccountClass.Cost);
            //ExpandTree(treeView5);

            this.treeView6.ItemsSource = GetNodeList(list, AccountClass.ProfitAndLoss);
            //ExpandTree(treeView6);

            this.treeView7.ItemsSource = GetNodeList(list, AccountClass.OffBalanceSheet);
            //ExpandTree(treeView7);
        }


        private void ExpandTree(TreeView treeView)
        {
            if (treeView.Items != null && treeView.Items.Count > 0)
            {
                foreach (var item in treeView.Items)
                {
                    DependencyObject dependencyObject = treeView.ItemContainerGenerator.ContainerFromItem(item);
                    if (dependencyObject != null)//第一次打开程序，dependencyObject为null，会出错
                    {
                        ((TreeViewItem)dependencyObject).ExpandSubtree();
                    }
                }
            }
        }

        Node ConvertToNode(AccountSubjectObj obj)
        {
            var node = new Node();
            node.Content = obj;
            node.Id = obj.id;
            node.DisplayName = string.Format("{0} - {1}",obj.no,obj.name);
            if (obj.isHasChild)
                node.NodeType = NodeType.StructureNode;
            else
                node.NodeType = NodeType.LeafNode;
            return node;
        }

        Node FindById(List<Node> list,long id)
        {
            foreach (var a in list)
            {
                if (a.Id == id)
                    return a;
                else if (a.Nodes != null && a.Nodes.Count > 0)
                {
                    Node node= FindById(a.Nodes, id);
                    if (node != null)
                        return node;
                }
            }
            return null;
        }

        private List<Node> GetNodeList(List<AccountSubjectObj> lst,AccountClass accountClass)
        {
            List<AccountSubjectObj> list = lst
                                        .FindAll(a=>a.accountClass== accountClass)
                                        .OrderBy(a=>a.level)
                                        .ToList();

            var lstResult = new List<Node>();
            list.ForEach(a=> {
                //Console.WriteLine(string.Format("{2}:{0} - {1}", a.no, a.name,a.id));
                if (a.level == 1)
                {
                    lstResult.Add(ConvertToNode(a));
                }
                else
                {
                    var node = FindById(lstResult, a.parentId); 
                    if(node != null)
                    {
                        if (node.Nodes == null)
                            node.Nodes = new List<Node>();
                        node.Nodes.Add(ConvertToNode(a));
                    }                    
                }
            });

            return lstResult;
        }

        private void treeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeView = sender as TreeView;
            var cur = treeView.SelectedItem as Node;
            if (cur != null && cur.NodeType != NodeType.StructureNode)
            {
                var args = new SelectedEventArgs { subjectObj = cur.Content, Handled = false };
                SelectedEvent?.Invoke(args);
                if (args.Handled)
                    this.Close();
            }
        }
    }
}
