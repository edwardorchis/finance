using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finance.Account.SDK
{
    public class AccountSubject
    {
        /// <summary>
        /// 内码
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 代码
        /// </summary>
        public string no { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 全称
        /// </summary>
        public string fullName { set; get; }
        /// <summary>
        /// 父节点
        /// </summary>
        public long parentId { set; get; }
        /// <summary>
        /// 根节点
        /// </summary>
        public long rootId { set; get; }
        /// <summary>
        /// 组ID
        /// </summary>
        public long groupId { set; get; }
        /// <summary>
        /// 第几层
        /// </summary>
        public int level { set; get; }
        /// <summary>
        /// 是否还有下层节点
        /// </summary>
        public bool isHasChild { set; get; }
        /// <summary>
        /// 余额方向
        /// </summary>
        public int direction { set; get; }
        /// <summary>
        /// 禁用
        /// </summary>
        public bool isDeleted { set; get; }
   
        
        public int flag { set; get; }

        public string actItemGrp { set; get; }

        public string actUint { set; get; }
    }
}
