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
            CreateSourceFile();
            var bRnt = CreateTableDef(tid);
            if (!bRnt)
                return;
            InitData(tid);
            logger.Info("Init data done.");
        }


        static void CreateSourceFile()
        {
            Generator.GenerateSql(typeof(SystemData), 0);
            Generator.GenerateSql(typeof(SDK.Voucher),1);
            Generator.GenerateSql(typeof(BaseData),2);
            Generator.GenerateSql(typeof(AccountData),3);
        }


        static bool CreateTableDef(long tid)
        {
            string path = Generator.getSourcePath() + "Script\\";
            List<string> files = FileHelper.GetFilesName(path, "*.sql");
            files.Sort();
            var ctx = new Dictionary<string, object> { { "Tid", tid } };
            string tmp = string.Empty;
            foreach (var file in files)
            {
                string sql = FileHelper.Read(file);                
                try
                {
                    var spe = new string[]{ "go\r\n" };
                    var arr = sql.Split(spe, StringSplitOptions.RemoveEmptyEntries).ToList();
                    arr.ForEach(s => {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            tmp = s;
                            DBHelper.GetInstance(ctx).ExecuteSql(s);
                        }
                            
                    });                    
                    logger.Info(file);
                }
                catch (Exception ex)
                {
                    logger.Error(file);
                    logger.Error(ex.Message);
                    logger.Error(tmp);
                    return false;
                }
            }
            return true;
        }

        static void InitData(long tid)
        {
            try
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
            catch (FinanceException e)
            {
                logger.Error(e.Message);
            }
        }

      
    }
}
