using Finance.Utils;
using Finance.Account.Source.DTL;
using System.Text;
using Finance.Account.Source.Struct;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Finance.Account.Source
{
    public class SourceMain
    {
        static ILogger logger = Logger.GetLogger(typeof(SourceMain));
        public static void Init(long tid)
        {
            var bRnt = CreateTableDef(tid);
            if (!bRnt)
                return;
            InitData(tid);
            ExecUpgradeSql(tid);
            logger.Info("Init data done.");
        }


        static bool CreateTableDef(long tid)
        {
            string sql = "";
            sql += Generator.GenerateSql(typeof(SystemData));
            sql += Generator.GenerateSql(typeof(SDK.Voucher));
            sql += Generator.GenerateSql(typeof(BaseData));
            sql += Generator.GenerateSql(typeof(AccountData));
            return execSql(tid, sql);
        }

        static bool execSql(long tid, string sql)
        {
            string tmp = string.Empty;
            try
            {
                var ctx = new Dictionary<string, object> { { "Tid", tid } };                
                var spe = new string[] { "go\r\n" };
                var arr = sql.Split(spe, StringSplitOptions.RemoveEmptyEntries).ToList();
                arr.ForEach(s => {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        tmp = s;
                        DBHelper.GetInstance(ctx).ExecuteSql(s);
                    }

                });
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + tmp);
            }
        }

        static bool ExecUpgradeSql(long tid)
        {
            string path = Generator.getSourcePath() + "Script\\";
            List<string> files = FileHelper.GetFilesName(path, "*.sql");
            files.Sort();
           
            foreach (var file in files)
            {
                string sql = FileHelper.Read(file);
                if (!execSql(tid, sql))
                {
                    return false;
                }
            }
            return true;
        }

        static void InitData(long tid)
        {
            ExcelImportor auxiliaryDtl = new ExcelImportor(tid, new AuxiliaryDTL());
            auxiliaryDtl.Import();

            ExcelImportor accountSubjectDtl = new ExcelImportor(tid, new AccountSubjectDTL());
            accountSubjectDtl.Import();

            ExcelImportor systemProfileDtl = new ExcelImportor(tid, new SystemProfileDTL());
            systemProfileDtl.Import();

            ExcelImportor balanceSheetDtl = new ExcelImportor(tid, new BalanceSheetDTL());
            balanceSheetDtl.Import();

            ExcelImportor profitSheetDTL = new ExcelImportor(tid, new ProfitSheetDTL());
            profitSheetDTL.Import();

            ExcelImportor cashflowSheetDTL = new ExcelImportor(tid, new CashflowSheetDTL());
            cashflowSheetDTL.Import();           
        }

      
    }
}
