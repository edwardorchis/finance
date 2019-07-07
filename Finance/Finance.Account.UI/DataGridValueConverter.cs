using Finance.Account.Data;
using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Finance.Account.UI
{
    internal class HideZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = 0M;
            if (value == null)
                return "";
            bool bSuc = decimal.TryParse(value.ToString(),out val);
            if (bSuc && val == 0)
            {
                return "";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null || value.ToString() == "")          
            {
                return 0M;
            }
            return value;
        }
    }


    internal class AccountSubjectGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = 0M;
            if (value == null)
                return "";
            bool bSuc = decimal.TryParse(value.ToString(), out val);
            if (bSuc)
            {
                List<Auxiliary> lst = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.AccountGroupList);
                var aux = lst.FirstOrDefault(a => a.id == val);
                if (aux != null)
                    return aux.name;
            }
            return "无效";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    

    internal class DirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = 0;
            if (value == null)
                return "";
            bool bSuc = int.TryParse(value.ToString(), out val);
            if (bSuc)
            {
                if (val == 1)
                    return "借";
                if (val == -1)
                    return "贷";               
            }
            return "无效";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
