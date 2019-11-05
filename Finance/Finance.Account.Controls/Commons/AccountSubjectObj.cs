using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Controls.Commons
{
    public class AccountSubjectObj
    {
        public AccountSubjectObj()
        {
            no = string.Empty;
            name = string.Empty;
        }
        public string FullName
        {
            get
            {
                if (id == 0L)
                    return string.Empty;
                else
                    return string.Format("{0} - {1}", no, fullName);
            }
        }

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
        public int balanceDirection { set; get; }
        /// <summary>
        /// 现金科目
        /// </summary>
        public bool isCashSubject { set; get; }
        /// <summary>
        /// 银行科目
        /// </summary>
        public bool isBankSubject { set; get; }
        /// <summary>
        /// 现金等价物
        /// </summary>
        public bool isCashEqulvalent { set; get; }
        /// <summary>
        /// 主表项目
        /// </summary>
        public long mainProjectId { set; get; }

        public bool isDeleted { set; get; }

        public int flag { set; get; }

        public string fullName { set; get; }

        /// <summary>
        /// 科目类型
        /// </summary>
        internal AccountClass accountClass { set; get; }


        public string actItemGrp { set; get; }

        public string actUint { set; get; }
    }


    public enum AccountClass
    {
        /// <summary>
        /// 资产
        /// </summary>
        Asset = 1,
        /// <summary>
        /// 负债
        /// </summary>
        Liability = 2,
        /// <summary>
        /// 共同
        /// </summary>
        Common = 3,
        /// <summary>
        /// 权益
        /// </summary>
        Equity = 4,
        /// <summary>
        /// 成本
        /// </summary>
        Cost = 5,
        /// <summary>
        /// 损益
        /// </summary>
        ProfitAndLoss = 6,
        /// <summary>
        /// 表外
        /// </summary>
        OffBalanceSheet = 7
    }
}
