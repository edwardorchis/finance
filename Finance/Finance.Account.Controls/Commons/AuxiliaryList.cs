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
        Max
    }

    public class AuxiliaryUtil
    {
        public static Dictionary<int, string> Types()
        {
            return new Dictionary<int, string>(){
                { (int)AuxiliaryType.AccountGroup,   "科目组"},
                { (int)AuxiliaryType.ProofOfWords,   "凭证字"},
                { (int)AuxiliaryType.AccountContent, "凭证摘要"},
                { (int)AuxiliaryType.AccountFlag, "科目掩码"}
            };
        }
    }


    /// <summary>
    /// 缓存基础数据，由VoucherGrid对外暴露接口写入
    /// </summary>
    public class AuxiliaryList
    {
        static AuxiliaryList cache = null;
        public static AuxiliaryList Instance
        {
            get
            {
                if (cache == null)
                {
                    cache = new AuxiliaryList();
                }
                return cache;
            }
        }

        public static List<AuxiliaryObj> Get(AuxiliaryType type)
        {
            var list = Instance.AuxiliaryObjects(type);
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

        List<AuxiliaryObj> AuxiliaryObjects(AuxiliaryType type)
        {
            var list = FinanceControlEventsManager.Instance.OnGetAuxiliaryObjListEvent(type);
            if (list == null)
                list = new List<AuxiliaryObj>();
            return list;
        }

    }
}
