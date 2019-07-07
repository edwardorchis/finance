using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finance.Account.Service.Utils
{
    internal class SampleCalculator
    {
        static ILogger logger = Logger.GetLogger(typeof(SampleCalculator));
        public static string sumLine(string formula, string key, Dictionary<string, string> result,bool suffix = false)
        {
            //logger.Debug(string.Format("{0}   {1}  ", key, formula));
            List<string> lstParams = CommonUtils.MatchPattern(formula, @"(\([^\(\)]*?\)|[^(^:]L[0-9]*)");
            List<string> lstOpratio = CommonUtils.MatchPattern(formula, "(\\+|\\-|\\*|/)");
            decimal sumAmount = 0M;
            for (int i = 0; i < lstParams.Count; i++)
            {
                decimal amount = 0M;
                var para = lstParams[i];
                if (para.StartsWith("("))
                {
                    para = para.Substring(1, para.Length - 2);
                    var tmp = para.Split(':');
                    var begin = int.Parse(tmp[0].Substring(1));
                    var end = int.Parse(tmp[1].Substring(1));
                    for (var j = begin; j <= end; j++)
                    {
                        var dKey = j + key.Substring(key.Length - 1);
                        if (result.ContainsKey(dKey))
                        {
                            decimal outd = 0M;
                            decimal.TryParse(result[dKey], out outd);
                            amount += outd;
                        }
                    }
                }
                else
                {
                    para = para.Substring(2);
                    var dKey = para + key.Substring(key.Length - 1);
                    if (result.ContainsKey(dKey))
                    {
                        decimal outd = 0M;
                        decimal.TryParse(result[dKey], out outd);
                        amount += outd;                        
                    }
                }
                if (i > 0)
                {                    
                    sumAmount = CalcOpration(sumAmount, lstOpratio[i - 1], amount);
                    //logger.Debug(string.Format("{0}   {1}    {2}", lstOpratio[i - 1], amount, sumAmount));
                }                 
                else
                    sumAmount = amount;
            }
            return sumAmount.ToString("0.00");
        }

        public static decimal CalcOpration(decimal prev, string opration, decimal amount)
        {
            decimal result = 0M;
            switch (opration)
            {
                case "+":
                    result = prev + amount;
                    break;
                case "-":
                    result = prev - amount;
                    break;
                case "*":
                    result = prev * amount;
                    break;
                case "/":
                    if (amount == 0)
                        result = prev;
                    else
                        result = prev / amount;
                    break;
            }
            return result;
        }

        public static List<long> GetAccountSubjectIds(List<AccountSubject> lstAso, string param)
        {
            var lstResult = new List<long>();
            if (param.Contains(":"))
            {
                var lst = param.Split(':');
                var begin = lst[0];
                var end = lst[1];
                var b = false;

                for (int i = 0; i < lstAso.Count; i++)
                {
                    var aso = lstAso[i];
                    if (aso.no == begin)
                        b = true;
                    if (b)
                        lstResult.Add(aso.id);
                    if (aso.no == end)
                    {
                        b = false;
                        break;
                    }
                }
            }
            else
            {
                var aso = lstAso.FirstOrDefault(a => a.no == param);
                if (aso != null)
                    lstResult.Add(aso.id);
            }
            return lstResult;
        }
    }
}
