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
        ///// <summary>
        ///// 现金科目
        ///// </summary>
        //public bool isCashSubject { set; get; }
        ///// <summary>
        ///// 银行科目
        ///// </summary>
        //public bool isBankSubject { set; get; }
        ///// <summary>
        ///// 现金等价物
        ///// </summary>
        //public bool isCashEqulvalent { set; get; }       
        ///// <summary>
        ///// 主表项目
        ///// </summary>
        //public long mainProjectId { set; get; }

        
        public int flag { set; get; }
    }
}
