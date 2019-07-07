using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Finance.Account.Service
{
    public  class AccountBalanceService
    {
        static ILogger logger = Logger.GetLogger(typeof(AccountBalanceService));     
        private IDictionary<string, object> mContext;
        public AccountBalanceService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static AccountBalanceService GetInstance(IDictionary<string, object> ctx)     
        {       
            return new AccountBalanceService(ctx);
        }
        /// <summary>
        /// 获取期间已结账期初余额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="perid"></param>
        /// <returns></returns>
        public List<AccountAmountItem> QuerySettled(long year,long perid)
        {
            var dt = DBHelper.GetInstance(mContext).ExecuteDt(string.Format("select * from _AccountBalance where _year={0} and _period= {1}",year,perid));
            if (dt == null || dt.Rows.Count == 0)
                return new List<AccountAmountItem>();
            var lst = EntityConvertor<AccountAmountItem>.ToList(dt);
            return lst;
        }
        /// <summary>
        /// 统计发生额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="perid"></param>
        /// <returns></returns>
        public List<AccountAmountItem> QueryOccurs(long beginYear, long beginPerid, long endYear, long endPerid)
        {
            VoucherService voucherService = new VoucherService(mContext);
            Dictionary<string, object> filter = new Dictionary<string, object>();
            filter.Add("beginYear", beginYear);
            filter.Add("beginPerid", beginPerid);
            filter.Add("endYear", endYear);
            filter.Add("endPerid", endPerid);
            var lst = voucherService.List(filter);
            Dictionary<long, AccountAmountItem> dict = new Dictionary<long, AccountAmountItem>();
            foreach (Voucher v in lst)
            {
                foreach (VoucherEntry entry in v.entries)
                {
                    if (!dict.ContainsKey(entry.accountSubjectId))
                    {
                        dict.Add(entry.accountSubjectId, new AccountAmountItem { accountSubjectId=entry.accountSubjectId});
                    }
                    if (entry.direction == 1)
                        dict[entry.accountSubjectId].debitsAmount += entry.amount;
                    else
                        dict[entry.accountSubjectId].creditAmount += entry.amount;
                }
            }
            return dict.Values.ToList();
        }


        public void Settle()
        {
            SystemProfileService system = new SystemProfileService(mContext);
            var year = system.GetInt(SystemProfileCategory.Account,SystemProfileKey.CurrentYear);
            var period = system.GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentPeriod);

            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                //1、获取id
                DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt(string.Format("select _id from _VoucherHeader where _year = {0} and _period = {1}",year,period));
                var lstIds = dt.AsEnumerable().Select(p => p.Field<long>("_id"));
                if (lstIds.Count() > 0)
                {
                    var ids = string.Join(",", lstIds);
                    //2、检查是否有未审核、未过账的，不能结账
                    var bExist = DBHelper.GetInstance(mContext).Exist(tran,
                        string.Format("select 1 from _VoucherHeader where _id in ({0}) and _status < {1}", ids, (int)VoucherStatus.Posted));
                    if (bExist)
                        throw new FinanceException(FinanceResult.INCORRECT_STATE);

                    //3、把结账到余额表
                    string.Format(@"
                        insert into _AccountBalance(_year,_period,_accountSubjectId,_debitsAmount,_creditAmount)
                        select {1},{2},_accountSubjectId,SUM(_debitsAmount) as _debitsAmount,SUM(_creditAmout) _creditAmout from (
                        select _accountSubjectId,_amount as _debitsAmount,0 as _creditAmout from _VoucherEntry where _id in ({0}) and _direction = 1
                        union
                        select _accountSubjectId,0 as _debitsAmount,_amount as _creditAmout from _VoucherEntry where _id in ({0}) and _direction = -1
                        ) t group by _accountSubjectId
                        ", ids,year,period);

                    //4、更新状态
                    DBHelper.GetInstance(mContext).ExecuteSql(tran, 
                        string.Format("update _VoucherHeader set _status = {0} where _id in ({1})", (int)VoucherStatus.Settled , ids));
                }
                //5、更新Current Period 为新的
                var next = CommonUtils.CalcNextPeriod(new PeridStrunct { Year = year, Period = period });
                system.Update(tran,SystemProfileCategory.Account, SystemProfileKey.CurrentYear, next.Year.ToString());
                system.Update(tran,SystemProfileCategory.Account, SystemProfileKey.CurrentPeriod, next.Period.ToString());

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
    }
}
