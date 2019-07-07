using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finance.Account.SDK
{
    public class VoucherEntry
    {
        /// <summary>
        /// 内码
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 分录id
        /// </summary>
        public long index { set; get; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public long accountSubjectId { set; get; }
        /// <summary>
        /// 科目代号
        /// </summary>
        public string accountSubjectNo { set; get; }
        ///// <summary>
        ///// 核算项目ID
        ///// </summary>
        //public long itemId { set; get; }
        ///// <summary>
        ///// 结算方式
        ///// </summary>
        //public long settleTypeId { set; get; }
        ///// <summary>
        ///// 结算号
        ///// </summary>
        //public string settleNo { set; get; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string explanation { set; get; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal amount { set; get; }
        /// <summary>
        /// 借贷方向
        /// </summary>
        public int direction { set; get; }

        public string uniqueKey { set; get; }
        /// <summary>
        /// 关联单号
        /// </summary>
        public string linkNo { set; get; }
    }
}
