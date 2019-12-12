using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UI
{
    public class UiUtils
    {
        public static Dictionary<string, string> FirstMenuDisplayNameMap = new Dictionary<string, string>
        {
            { "base_setting", "基础设置" },
            { "account", "财务管理"},
            { "other", "其他功能"},
            { "report", "财务报表"}
        };
       
    }
}
