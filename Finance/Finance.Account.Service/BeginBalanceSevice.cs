using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public  class BeginBalanceSevice
    {
        static ILogger logger = Logger.GetLogger(typeof(BeginBalanceSevice));
        private IDictionary<string, object> mContext;
        public BeginBalanceSevice(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static BeginBalanceSevice GetInstance(IDictionary<string, object> ctx)
        {
            return new BeginBalanceSevice(ctx);
        }

        public void Save(List<BeginBalance> balances)
        {
            var totalDebitsAmount = balances.Sum(b=>b.debitsAmount);
            var totalCreditAmount = balances.Sum(b => b.creditAmount);

            if (totalDebitsAmount != totalCreditAmount)
                throw new FinanceException(FinanceResult.AMMOUNT_IMBALANCE);

            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                DataTable dt = EntityConvertor<BeginBalance>.ToDataTable(balances);
                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from _BeginBalance");
                DBHelper.GetInstance(mContext).InsertTable(tran,dt, "_BeginBalance");
                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (FinanceException ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }
            catch (Exception e)
            {
               
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                var traceId = SerialNoService.GetUUID();
                logger.Error(e, traceId);
                throw new FinanceException(FinanceResult.SYSTEM_ERROR, traceId);
            }               
        }
        public List<BeginBalance> List()
        {
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt("select * from _beginBalance");
            var lst= EntityConvertor<BeginBalance>.ToList(dt);
            return lst;
        }

        public void Finish()
        {
            SystemProfileService systemProfileService = SystemProfileService.GetInstance(mContext);
            var startPeriod = new PeridStrunct
            {
                Year = systemProfileService.GetInt(SystemProfileCategory.Account, SystemProfileKey.StartYear),
                Period = systemProfileService.GetInt(SystemProfileCategory.Account, SystemProfileKey.StartPeriod)
            };
            
            PeridStrunct prev = CommonUtils.CalcPrevPeriod(startPeriod);
            DBHelper.GetInstance(mContext).ExecuteSql(string.Format(@"insert into _AccountBalance (_year,_period,_accountSubjectId,_debitsAmount,_creditAmount)  
            select {0},{1},_accountSubjectId,_debitsAmount,_creditAmount from _BeginBalance ",prev.Year,prev.Period));

            systemProfileService.Update(SystemProfileCategory.Account, SystemProfileKey.IsInited, "1");

        }
     }
}
