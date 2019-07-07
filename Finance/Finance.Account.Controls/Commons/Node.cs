using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Controls.Commons
{
    public enum NodeType
    {
        RootNode,//根节点
        LeafNode,//叶子节点
        StructureNode//结构节点，仅起到组织配置文件结构的作用，不参与修改
    }
    public class Node
    {        
        public string Icon { set; get; }
        /// <summary>
        /// 节点ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///  节点名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 节点携带的内容
        /// </summary>
        public AccountSubjectObj Content { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<Node> Nodes { get; set; }
    }
}
