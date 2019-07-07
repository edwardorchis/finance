using Finance.Account.SDK;
using Finance.Account.Service.Utils;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Finance.Account.Service
{
    public  class BalanceSheetService
    {
        static ILogger logger = Logger.GetLogger(typeof(BalanceSheetService));
        private IDictionary<string, object> mContext;
        public BalanceSheetService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static BalanceSheetService GetInstance(IDictionary<string, object> ctx)
        {
            return new BalanceSheetService(ctx);
        }

        List<AccountAmountItem> m_lstBegin = null;
        List<AccountAmountItem> m_lstOccurs = null;
        List<AccountSubject> m_lstAso = null;

        public Dictionary<string, string> Calc(IDictionary<string, object> filter, IDictionary<string, string> template)
        {
            var beginYear = int.Parse(filter["beginYear"].ToString());
            var beginPeriod = int.Parse(filter["beginPeriod"].ToString());
            var endYear = int.Parse(filter["endYear"].ToString());
            var endPeriod = int.Parse(filter["endPeriod"].ToString());

            AccountBalanceService abService = AccountBalanceService.GetInstance(mContext);
            var prev = CommonUtils.CalcPrevPeriod(new PeridStrunct { Year = beginYear, Period = beginPeriod });
            m_lstBegin = abService.QuerySettled(prev.Year, prev.Period);
            m_lstOccurs = abService.QueryOccurs(beginYear, beginPeriod, endYear, endPeriod);
            m_lstAso = DataManager.GetInstance(mContext).Query<AccountSubject>(null).OrderBy(a => a.no).ToList();

            Dictionary<string, string> result = new Dictionary<string, string>();
            List<string> sumKeys = new List<string>();
            foreach (KeyValuePair<string, string> kv in template)
            {
                if (Regex.IsMatch(kv.Value, "(L)"))
                {
                    sumKeys.Add(kv.Key);
                    continue;
                }                
                var rnt = Calc(kv.Value);
                result.Add(kv.Key,rnt);                
                logger.Debug("{0} {1} {2}",kv.Key,rnt, kv.Value);
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

            List<string> lstMethod = CommonUtils.MatchPattern(formula, "(Y|C|JY|DC|DY)");
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
                var ids = SampleCalculator.GetAccountSubjectIds(m_lstAso,param);
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
                case "Y":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount - a.creditAmount; })
                            + CalcSum(ids, m_lstOccurs, (a) => { return a.debitsAmount - a.creditAmount; });
                    break;
                case "JY":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount; })
                            + CalcSum(ids, m_lstOccurs, (a) => { return a.debitsAmount; });
                    break;
                case "DY":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.creditAmount; })
                            + CalcSum(ids, m_lstOccurs, (a) => { return a.creditAmount; });
                    break;
                case "C":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount - a.creditAmount; });
                    break;
                case "JC":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount; });
                    break;
                case "DC":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.creditAmount; });
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
