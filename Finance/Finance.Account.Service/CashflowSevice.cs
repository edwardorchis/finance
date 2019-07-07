
using Finance.Account.SDK;
using Finance.Account.Service.Model;
using Finance.Account.Service.Utils;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finance.Account.Service
{
    public class CashflowSevice
    {
        static ILogger logger = Logger.GetLogger(typeof(CashflowSevice));
        private IDictionary<string, object> mContext;
        public CashflowSevice(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static CashflowSevice GetInstance(IDictionary<string, object> ctx)
        {
            return new CashflowSevice(ctx);
        }

        List<AccountAmountItem> m_lstBegin = null;
        List<AccountAmountItem> m_lstYear = null;
        List<AccountAmountItem> m_lstOccurs = null;
        List<AccountSubject> m_lstAso = null;
        public List<CashflowSheetItem> ListSheet(Dictionary<string, string> filter)
        {
            List<ExcelTemplateItem> lstTemplate = TemplateSevice.GetInstance(mContext).FindTemplate("现金流量表");
            var result = new List<CashflowSheetItem>();

            var beginYear = int.Parse(filter["beginYear"]);
            var beginPeriod = int.Parse(filter["beginPeriod"]);
            var endYear = int.Parse(filter["endYear"]);
            var endPeriod = int.Parse(filter["endPeriod"]);
            var prev = CommonUtils.CalcPrevPeriod(new PeridStrunct { Year = beginYear, Period = beginPeriod });
            var curYear = SystemProfileService.GetInstance(mContext).GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentYear);

            m_lstAso = DataManager.GetInstance(mContext).Query<AccountSubject>(null).OrderBy(a => a.no).ToList();
            m_lstBegin = AccountBalanceService.GetInstance(mContext).QuerySettled(prev.Year, prev.Period);
            m_lstOccurs = AccountBalanceService.GetInstance(mContext).QueryOccurs(beginYear, beginPeriod, endYear, endPeriod);
            m_lstYear = AccountBalanceService.GetInstance(mContext).QueryOccurs(curYear, 1, curYear, 12);

            Dictionary<int, CalTempObj> dictTemplate = new Dictionary<int, CalTempObj>();
            foreach (var template in lstTemplate)
            {
                var item = new CashflowSheetItem();
                item.Name = template.a;
                item.LineNo = template.b;   
                var lineNo = 0;
                if (int.TryParse(item.LineNo, out lineNo))
                {
                    item.Amount = CalcFormula(lineNo, template.c);                   
                    dictTemplate.Add(lineNo, new CalTempObj(template,item.Amount));                   
                }
                result.Add(item);
            }

            foreach (var item in result)
            {
                //计算扩展列
                var lineNo = 0;
                if (int.TryParse(item.LineNo, out lineNo))
                {
                    decimal extendAmount = 0;
                    if (CalcExtend(lineNo, dictTemplate, out extendAmount))
                    {
                        item.Amount = extendAmount;                        
                    }
                }
            }

            foreach (var item in result)
            {
                //计算合计列L
                var lineNo = 0;
                if (int.TryParse(item.LineNo, out lineNo))
                {
                    decimal sumAmount = 0;
                    if (CalcSum(lineNo, dictTemplate, out sumAmount))
                    {
                        item.Flag = 1;
                        item.Amount = sumAmount;
                        dictTemplate[lineNo].originAmount = item.Amount;
                    }
                }
                
            }
            return result;
        }

        //'=(?L2>0)+ABS(?L7<0)
        bool CalcExtend(int lineNo,Dictionary<int,CalTempObj> dict,out decimal amount)
        {
            bool result = false;
            amount = 0;

            if (!dict.ContainsKey(lineNo))
            {
                logger.Debug("dict don't have key:{0}", lineNo);
                return result;
            }

            var strExtend = dict[lineNo].templateItem.d;
            if (string.IsNullOrEmpty(strExtend) || !strExtend.StartsWith("="))
                return result;
            
            //获取括号之间的内容
            List<string> lstParams = CommonUtils.MatchPattern(strExtend, "(?<=\\()[^\\)]+");
            result = lstParams.Count>0;
            foreach (var exp in lstParams)
            {                
                if (exp.StartsWith("?"))
                {
                    var pos = exp.IndexOf('<') + exp.IndexOf('>');
                    if (pos != -1)
                    {
                        var row = exp.Substring(2, pos - 1);
                        int tmpRow = 0;
                        if (int.TryParse(row, out tmpRow))
                        {
                            if(dict.ContainsKey(tmpRow))
                            {
                                if (!dict.ContainsKey(tmpRow))
                                {
                                    logger.Debug("dict don't have key:{0}", tmpRow);
                                    continue;
                                }
                                var tmpAmount = dict[tmpRow].originAmount;
                                if ((tmpAmount < 0 && exp.Substring(pos, 1) == "<")//get
                                    || (tmpAmount > 0 && exp.Substring(pos, 1) == "<"))
                                {
                                    amount += tmpAmount;
                                }                               
                            }                           
                        }
                    }
                }
            }
            logger.Debug("CalcExtend : [{0}]{1}{2}", lineNo, amount, strExtend);
            return result;
        }

        //'=L4+L5+L6+L7
        bool CalcSum(int lineNo, Dictionary<int, CalTempObj> dict,out decimal amount)
        {
            var result = false;
            amount = 0;
            if (!dict.ContainsKey(lineNo))
            {
                logger.Debug("dict don't have key:{0}",lineNo);
                return result;
            }
                
            var expression = dict[lineNo].templateItem.c;
            List<string> lstParams = CommonUtils.MatchPattern(expression, @"L([0-9]+)");
            List<string> lstOpratio = CommonUtils.MatchPattern(expression, "(\\+|\\-|\\*|/)");
            var size = lstParams.Count;
            if (size == 0)
                return result;
            
            if (lstOpratio.Count + 1 != size)
            {
                logger.Error("error expression:" + expression);
                return result;
            }

            result = size > 0;

            for (int i = 0; i < size; i++)
            {
                var rowStr = lstParams[i].Substring(1);
                var row = 0;
                if (int.TryParse(rowStr, out row))
                {
                    if (!dict.ContainsKey(row))
                    {
                        logger.Debug("dict don't have key:{0}", row);
                        continue;
                    }
                    var tmp = dict[row].originAmount;
                    if (i > 0)
                        amount = SampleCalculator.CalcOpration(amount, lstOpratio[i - 1], tmp);
                    else
                        amount = tmp;
                }            
            }
            logger.Debug("calcSum : [{0}]{1}{2}", lineNo, amount, expression);
            return result;
        }

        //'=SY("6301")-SY("1221")
        decimal CalcFormula(int lineNo,string formula)
        {
            if (string.IsNullOrEmpty(formula))
                return 0;
            if (!formula.StartsWith("="))
                return 0;

            List<string> lstMethod = CommonUtils.MatchPattern(formula, "(SY|C|SJY|SL)");
            if (lstMethod.Count == 0)
                return 0;
            
            List<string> lstParams = CommonUtils.MatchPattern(formula, "(?<=\")[^\"^(^)]*(?=\")");
            List<string> lstOpratio = CommonUtils.MatchPattern(formula, "(\\+|\\-|\\*|/)");           

            if (lstMethod.Count != lstParams.Count)
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

            logger.Debug("CalcFormula : [{0}]{1}{2}", lineNo,result ,formula);
            return result;
        }

        decimal CalcMethod(string method, List<long> ids)
        {
            decimal amount = 0M;
            switch (method)
            {
                case "SY":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount - a.creditAmount; })
                            + CalcSum(ids, m_lstOccurs, (a) => { return a.debitsAmount - a.creditAmount; });
                    break;
                case "SJY":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount; })
                            + CalcSum(ids, m_lstOccurs, (a) => { return a.debitsAmount; });
                    break;             
                case "C":
                    amount = CalcSum(ids, m_lstBegin, (a) => { return a.debitsAmount - a.creditAmount; });
                    break;
                case "SL":
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
                    .Sum(a => m_lstAso.FirstOrDefault(aso=>aso.id ==id ).direction * func(a));
            }
            return result;
        }

        class CalTempObj
        {
            public CalTempObj(ExcelTemplateItem item,decimal amount)
            {
                templateItem = item;
                originAmount = amount;
            }

            public ExcelTemplateItem templateItem { set; get; }
            public decimal originAmount { set; get; }

        }
    }
}
