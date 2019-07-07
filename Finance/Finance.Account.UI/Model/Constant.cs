using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.UI.Model
{
    public static class Constant
    {
        public delegate void FilterPopupEventHandler(FilterPopupEventArgs e);
        public delegate void ShowSelectedItemEventHandler(long id);

        public static Dictionary<VoucherStatus, string> VoucherStatusDictionary = new Dictionary<VoucherStatus, string>()
        {
            { VoucherStatus.Invalid,    "无效"},
            { VoucherStatus.Normal,    ""},
            { VoucherStatus.Checked,    "已审核"},
            { VoucherStatus.Canceled,    "已作废"},
            { VoucherStatus.Posted,    "已过账"},
            { VoucherStatus.Settled,    "已结账"},
        };
    }
    /// <summary>
    /// 表的模式
    /// </summary>
    public enum SheetModel
    {
        DEFAULT,
        DATA,               //数据
        FORMULA,            //公式
    }
}
