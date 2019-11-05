using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Controls.Commons
{
    /// <summary>
    /// 辅助资料
    /// </summary>
    public class AuxiliaryObj
    {
        /// <summary>
        /// 内码
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public long type { set; get; }
        /// <summary>
        /// 代码
        /// </summary>
        public string no { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { set; get; }
        /// <summary>
        /// 父节点
        /// </summary>
        public long parentId { set; get; }
        /// <summary>
        /// 分组
        /// </summary>
        public int groupId { set; get; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool isUnused { set; get; }
    }
}
