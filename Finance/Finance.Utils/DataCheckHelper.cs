using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Utils
{
    public static class DataCheckHelper
    {
        public static void StringIsNullOrEmpty(string value, string msg = "")
        {
            if (string.IsNullOrEmpty(value))
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, msg);
        }

        public static void NumberIsZero(dynamic value, string msg = "")
        {
            if (value == 0)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, msg);
        }

        public static void IsNull(dynamic value, string msg = "")
        {
            if (value == null)
                throw new FinanceException(FinanceResult.NULL, msg);
        }

        public static void DateInPeriod(DateTime date, int year, int period, string msg = "")
        {
            if(date.Year!=year || date.Month !=period)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, msg);
        }

    }
}
