using Finance.Account.Data;
using Finance.Account.SDK;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Media;

namespace FinanceClient
{



    public partial class FormMain
    {
        string APP_CONFIG_KEY_BORDERBRUSH = "theme";

        string MENU_LIST_PREFIX = "list_";
        string SUB_TAB_PREFIX = "tab_";

        string FINANCE_TITLE = "中威智能财务软件";

        //List<MenuTableMapItem> menuTableMap = new List<MenuTableMapItem> {
        //    new MenuTableMapItem("base_setting","user_list","用户列表","FormUser"),
        //    new MenuTableMapItem("base_setting","account_subject","科目","FormAccountSubject"),
        //    new MenuTableMapItem("base_setting","auxiliary_manager","辅助资料","FormAuxiliary"),

        //    new MenuTableMapItem("account","begin_balance","初始余额表","FormBeginBalance"),
        //    new MenuTableMapItem("account","voucher_input","凭证录入","FormVoucher"),
        //    new MenuTableMapItem("account","voucher_list","凭证列表","FormVoucherList"),
        //    new MenuTableMapItem("account","account_balance","科目余额表","FormAccountBalance"),
        //    new MenuTableMapItem("account","cashflow_sheet","现金流量表","FormCashflowSheet"),
        //    new MenuTableMapItem("account","balance_sheet","资产负债表","FormBalanceSheet"),
        //    new MenuTableMapItem("account","profit_sheet","利润表","FormProfitSheet"),
        //    new MenuTableMapItem("account","settle","结账",""),
        //    new MenuTableMapItem("account","sp_udefreport_voucher_udef","数量统计表","FormUdefReport"),
        //};

        List<MenuTableMap> menuTableMap
        {
            get {
                var lst = DataFactory.Instance.GetSystemProfileExecuter().GetMenuTables();
                return lst;
            }
        }



        Brush HEARBEATTIMEOUT_BRUSH = new SolidColorBrush(Color.FromRgb(255, 0, 0));
    }
}
