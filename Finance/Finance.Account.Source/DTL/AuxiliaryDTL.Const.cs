using Finance.Utils;
using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Finance.Account.Source.DTL
{
    public partial class AuxiliaryDTL : IImportHandler
    {
        static IDictionary<string, AccountClass> AccountClassMap = new Dictionary<string, AccountClass> {
            {"资产", AccountClass.Asset},
            {"负债", AccountClass.Liability},
            {"共同", AccountClass.Common},
            {"权益", AccountClass.Equity},
            {"成本", AccountClass.Cost},
            {"损益", AccountClass.ProfitAndLoss},
            {"表外", AccountClass.OffBalanceSheet},

        };
        static IDictionary<string, AuxiliaryType> AuxiliaryTypeMap = new Dictionary<string, AuxiliaryType>
        {
            {"科目组", AuxiliaryType.AccountGroup},
            //{"现金流量主表项目", AuxiliaryType.CashFlowItem},
            //{"现金流量附表项目", AuxiliaryType.CashFlowAttachedItem },
            {"凭证字", AuxiliaryType.ProofOfWords},
        };


    }
}
