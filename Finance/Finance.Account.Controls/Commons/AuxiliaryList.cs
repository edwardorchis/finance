using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Account.Controls.Commons.Consts;

namespace Finance.Account.Controls.Commons
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
        CarriedForwardTemplate,
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


    /// <summary>
    /// 缓存基础数据，由VoucherGrid对外暴露接口写入
    /// </summary>
    public class AuxiliaryList
    {
        public static List<AuxiliaryObj> Get(AuxiliaryType type)
        {
            var list = FinanceControlEventsManager.Instance.OnGetAuxiliaryObjListEvent();
            if (list == null)
                return new List<AuxiliaryObj>();
            list = list.FindAll(a => a.type == (int)type);
            if (list == null)
                return new List<AuxiliaryObj>();
            return list;
        }

        public static List<AuxiliaryObj> Get(int type)
        {
            var list = FinanceControlEventsManager.Instance.OnGetAuxiliaryObjListEvent();
            if (list == null)
                return new List<AuxiliaryObj>();
            list = list.FindAll(a => a.type == type);
            if (list == null)
                return new List<AuxiliaryObj>();
            return list;
        }

        public static AuxiliaryObj Find(AuxiliaryType type, long accountSubjectId)
        {
            var accountSubjectObj = Get(type).FirstOrDefault(item => item.id == accountSubjectId);
            if (accountSubjectObj == null)
            {
                accountSubjectObj = new AuxiliaryObj();
            }
            return accountSubjectObj;
        }

        public static AuxiliaryObj FindByNo(AuxiliaryType type, string no)
        {
            var accountSubjectObj = Get(type).FirstOrDefault(item => item.no == no);
            if (accountSubjectObj == null)
            {
                accountSubjectObj = new AuxiliaryObj();
            }
            return accountSubjectObj;
        }



        public static Dictionary<int, string> GetGroupTypes(AuxiliaryGroup auxGrp)
        {
            var dict = new Dictionary<int, string>();            
            var listGroup = Get(AuxiliaryType.Invalid).FindAll(a=>a.groupId == (int)auxGrp);
            if (listGroup == null)
                return dict;
            listGroup.ForEach(t=> {                
                    dict[int.Parse(t.no)] = t.name;
            });
            return dict;
        }

    }
}
