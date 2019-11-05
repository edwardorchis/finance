using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{ 
    public enum AuxiliaryType
    {
        Invalid = 0,
        /// <summary>
        /// 科目组
        /// </summary>
        AccountGroup = 1,
        /// <summary>
        /// 凭证字
        /// </summary>
        ProofOfWords,
        /// <summary>
        /// 凭证摘要
        /// </summary>
        AccountContent,
        /// <summary>
        /// 凭证掩码
        /// </summary>
        AccountFlag,
        /// <summary>
        /// 结转方式
        /// </summary>
        CarriedForward = 5,
        /// <summary>
        /// 供应商
        /// </summary>
        Supplier,
        /// <summary>
        /// 客户
        /// </summary>
        Customer,
        /// <summary>
        /// 产品
        /// </summary>
        Product,
        /// <summary>
        /// 其他附加项
        /// </summary>
        OtherItems,

        Max
    }

    public enum AuxiliaryGroup
    {
        Auxiliary,
        Reserve,
        AccountItems
    }

    public enum SerialNoKey
    {
        /// <summary>
        /// 系统
        /// </summary>
        System,
        /// <summary>
        /// 凭证号
        /// </summary>
        VoucherNo,
        /// <summary>
        /// 凭证序号
        /// </summary>
        VoucherSn,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }


    /// <summary>
    /// 凭证的状态
    /// </summary>
    public enum VoucherStatus
    {
        /// <summary>
        /// 无效的
        /// </summary>
        Invalid = -1,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        Checked,
        /// <summary>
        /// 已过账
        /// </summary>
        Posted,
        /// <summary>
        /// 已结账
        /// </summary>
        Settled,


        /// <summary>
        /// 已作废
        /// </summary>
        Canceled = 255,
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

    public enum OrderByRule
    {
        ESC,
        DESC
    }


    public enum SystemProfileCategory
    {
        Invalid = 0,
        /// <summary>
        /// 总账
        /// </summary>
        Account,
    }


    public enum SystemProfileKey
    {
        Invalid = 0,
        /// <summary>
        /// 当前会计年度
        /// </summary>
        CurrentYear,
        /// <summary>
        /// 当前会计期间
        /// </summary>
        CurrentPeriod,
        /// <summary>
        /// 开启会计年度
        /// </summary>
        StartYear,
        /// <summary>
        /// 开启会计期间
        /// </summary>
        StartPeriod,
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        IsInited,
    }

    public enum ExecTaskType
    {
        /// <summary>
        /// 创建凭证
        /// </summary>
        CreateVoucher,
        /// <summary>
        /// 月度结转
        /// </summary>
        CarriedForward
    }
}
