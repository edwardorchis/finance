using System;

namespace Finance.Account.SDK
{
    public class VoucherHeader
    {
        /// <summary>
        /// 内码
        /// </summary>
        public long id { set; get; }
        /// <summary>
        /// 凭证字
        /// </summary>
        public string word { set; get; }
        /// <summary>
        /// 凭证号
        /// </summary>
        public long no { set; get; }
        /// <summary>
        /// 序列号
        /// </summary>
        public long serialNo { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { set; get; }
        /// <summary>
        /// 参考信息
        /// </summary>
        public string reference { set; get; }
        /// <summary>
        /// 年度
        /// </summary>
        public int year { set; get; }
        /// <summary>
        /// 期间
        /// </summary>
        public int period { set; get; }
        /// <summary>
        /// 业务日期
        /// </summary>
        public DateTime businessDate { set; get; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime date { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime creatTime { set; get; }        
        /// <summary>
        /// 制单人
        /// </summary>
        public long creater { set; get; }
        /// <summary>
        /// 出纳
        /// </summary>
        public string cashier { set; get; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string agent { set; get; }


        /// <summary>
        /// 过账人
        /// </summary>
        public long poster { set; get; }
        /// <summary>
        /// 审核人
        /// </summary>
        public long checker { set; get; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? checkTime { set; get; }
        /// <summary>
        /// 过账时间
        /// </summary>
        public DateTime? postTime { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public int status { set; get; }

        /// <summary>
        /// 关联单号，辅助生成凭证，不存表
        /// </summary>
        public string linkNo { set; get; }
    }
}
