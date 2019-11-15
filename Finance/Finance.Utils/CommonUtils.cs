using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finance.Utils
{
    public static class CommonUtils
    {
        public static long TotalMilliseconds()
        {          
            DateTime d2 = new DateTime(1970, 1, 1);
            double d = DateTime.Now.Subtract(d2).TotalMilliseconds;
            return (long)d;
        }


        public static DateTime CalcMaxPeriodDate(int year,int period)
        {
            PeridStrunct periods = new PeridStrunct { Year = year,Period = period };
            PeridStrunct next_periods = CalcNextPeriod(periods);
            DateTime next_mouth_first_day = new DateTime(next_periods.Year, next_periods.Period, 1);
            return next_mouth_first_day.AddDays(-1);
        }

        public static PeridStrunct CalcNextPeriod(PeridStrunct period)
        {
            PeridStrunct next = new PeridStrunct();
            if (period.Period == 12)
            {
                next.Year = period.Year + 1;
                next.Period = 1;
            }
            else
            {
                next.Year = period.Year;
                next.Period = period.Period + 1;
            }

            return next;
        }
        public static PeridStrunct CalcPrevPeriod(PeridStrunct period)
        {
            PeridStrunct perv = new PeridStrunct();
            if (period.Period == 1)
            {
                perv.Year = period.Year - 1;
                perv.Period = 12;
            }
            else
            {
                perv.Year = period.Year;
                perv.Period = period.Period - 1;
            }

            return perv;
        }



        public static int TryParseInt(string str)
        {
            int result = 0;
            int.TryParse(str, out result);
            return result;
        }



        public static Type ConvertDataTypeFromStr(string dataType)
        {
            Type t = typeof(string);
            switch (dataType)
            {
                case "decimal":
                    t = typeof(decimal);
                    break;
                case "int":
                    t = typeof(int);
                    break;
            }

            return t;
        }

        public static bool StringNotEqNullAndWhiteSpace(string a,string b)
        {
            a = string.IsNullOrWhiteSpace(a) ? "" : a;
            b = string.IsNullOrWhiteSpace(b) ? "" : b;
            return a != b;
        }


        public static List<string> MatchPattern(string formula, string pattern)
        {
            var lst = new List<string>();
            foreach (Match match in Regex.Matches(formula, pattern))
            {
                if (match == null)
                    continue;
                string opration = match.Value;
                lst.Add(opration);
            }
            return lst;
        }
        public static int ToTimestamp(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (int)span.TotalSeconds;
        }

        public static string GetUUID()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }


    public class PeridStrunct
    {
        public int Year { set; get; }
        public int Period { set; get; }
    }
}
