using Finance.Account.SDK;
using Finance.Account.Service.Utils;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public  class ProfitSheetService
    {
        static ILogger logger = Logger.GetLogger(typeof(ProfitSheetService));
        private IDictionary<string, object> mContext;
        public ProfitSheetService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static ProfitSheetService GetInstance(IDictionary<string, object> ctx)
        {
            return new ProfitSheetService(ctx);
        }


        List<AccountAmountItem> m_lstYear = null;
        List<AccountAmountItem> m_lstOccurs = null;
        List<AccountSubject> m_lstAso = null;

        public List<ProfitSheetItem> ListSheet(Dictionary<string, string> filter)
        {

            return null;
        }


        public Dictionary<string, string> Calc(Dictionary<string, string> template)
        {
            SystemProfileService systemProfile = SystemProfileService.GetInstance(mContext);
            var curYear = systemProfile.GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentYear);
            var curPeriod = systemProfile.GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentPeriod);

            AccountBalanceService abService = AccountBalanceService.GetInstance(mContext);            
            m_lstYear = abService.QueryOccurs(curYear, 1, curYear, 12);
            m_lstOccurs = abService.QueryOccurs(curYear, curPeriod, curYear, curPeriod);
            m_lstAso = DataManager.GetInstance(mContext).Query<AccountSubject>(null).OrderBy(a => a.no).ToList();

            Dictionary<string, string> result = new Dictionary<string, string>();
            List<string> sumKeys = new List<string>();
            foreach (KeyValuePair<string, string> kv in template)
            {
                if (Regex.IsMatch(kv.Value, "(L)") && ! Regex.IsMatch(kv.Value, "(SL)"))
                {
                    sumKeys.Add(kv.Key);
                    continue;
                }
                result.Add(kv.Key, Calc(kv.Value));
            }

            sumKeys.Sort();
            foreach (string key in sumKeys)
            {
                result.Add(key, SampleCalculator.sumLine(template[key], key, result));
            }

            return result;
        }

        string Calc(string formula)
        {
            if (string.IsNullOrEmpty(formula))
                return "";
            if (!formula.StartsWith("="))
                return formula;

            List<string> lstMethod = CommonUtils.MatchPattern(formula, "(SY|SL)");
            List<string> lstParams = CommonUtils.MatchPattern(formula, "(?<=\")[^\"^(^)]*(?=\")");
            List<string> lstOpratio = CommonUtils.MatchPattern(formula, "(\\+|\\-|\\*|/)");

            if (lstMethod.Count != lstParams.Count)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "公式错误");
            if (lstMethod.Count != lstOpratio.Count + 1)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "公式错误");

            decimal result = 0M;
            for (int i = 0; i < lstMethod.Count; i++)
            {
                var param = lstParams[i];
                var ids = SampleCalculator.GetAccountSubjectIds(m_lstAso, param);
                if (ids.Count == 0)
                    continue;
                var method = lstMethod[i];
                var amount = CalcMethod(method, ids);
                if (i > 0)
                    result = SampleCalculator.CalcOpration(result, lstOpratio[i - 1], amount);
                else
                    result = amount;
            }
            return result.ToString("0.00");
        }




        decimal CalcMethod(string method, List<long> ids)
        {
            decimal amount = 0M;
            switch (method)
            {
                case "SY"://本期
                    amount = CalcSum(ids, m_lstOccurs, (a) => { return a.debitsAmount - a.creditAmount; });
                    break;
                case "SL"://本年
                    amount = CalcSum(ids, m_lstYear, (a) => { return a.debitsAmount - a.creditAmount; });
                    break;
            }
            return amount;
        }

        decimal CalcSum(List<long> ids, List<AccountAmountItem> lst, Func<AccountAmountItem, decimal> func)
        {
            decimal result = 0M;
            foreach (var id in ids)
            {
                result += lst.FindAll(a => a.accountSubjectId == id)
                    .Sum(a => m_lstAso.FirstOrDefault(aso => aso.id == id).direction * func(a));
            }
            return result;
        }


    }
}
