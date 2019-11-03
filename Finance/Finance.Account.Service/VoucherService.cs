using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Finance.Account.Service
{
    public class VoucherService
    {
        static ILogger logger = Logger.GetLogger(typeof(VoucherService));
        private IDictionary<string, object> mContext;
        public VoucherService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static VoucherService GetInstance(IDictionary<string, object> ctx)
        {
            return new VoucherService(ctx);
        }


        public long Update(Voucher item)
        {
            var id = item.header.id;
            var bExist = DBHelper.GetInstance(mContext).Exist("select * from _VoucherHeader where _id = " + id);
            if (!bExist)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);

            item.entries.ForEach(entry=>entry.id=id);

            CheckData(item);

            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                //已审核不能更新
                bExist = DBHelper.GetInstance(mContext).Exist(tran,"select * from _VoucherHeader where _status > 0 and _id = " + id);
                if (bExist)
                    throw new FinanceException(FinanceResult.INCORRECT_STATE);

                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from _VoucherEntry where _id= " + id);
                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from _VoucherEntryUdef where _id= " + id);
                DBHelper.GetInstance(mContext).ExecuteSql(tran, DataManager.GetInstance(mContext).BuildUpdateSql(item.header));
                DBHelper.GetInstance(mContext).InsertTable(tran,
                    EntityConvertor<VoucherEntry>.ToDataTable(item.entries), "_VoucherEntry");
                saveUdefEntry(item, tran);
                DBHelper.GetInstance(mContext).CommitTransaction(tran);
                return id;
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

        public long Add(Voucher item)
        {
            CheckData(item);

            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                GeneratorIds(item.header,tran);
                item.entries.ForEach(v => {
                    v.id = item.header.id;
                    if (v.accountSubjectId == 0 || !string.IsNullOrEmpty(v.accountSubjectNo)) 
                    {
                        var aso = AccountSubjectService.GetInstance(mContext).FindByNo(v.accountSubjectNo);
                        if (aso == null)
                            throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, string.Format("科目[{0}]不存在",v.accountSubjectNo));
                        v.accountSubjectId = aso.id;

                        if (aso.flag != 0 && !item.udefenties.ContainsKey(v.uniqueKey))
                        {
                            throw new FinanceException(FinanceResult.IMPERFECT_DATA, string.Format("科目[{0}]必须要有自定义扩展信息", v.accountSubjectNo));
                        }
                    }                        
                });

                DBHelper.GetInstance(mContext).InsertTable(tran,
                    EntityConvertor<VoucherHeader>.ToDataTable(new List<VoucherHeader> { item.header }), "_VoucherHeader");
                DBHelper.GetInstance(mContext).InsertTable(tran,
                    EntityConvertor<VoucherEntry>.ToDataTable(item.entries), "_VoucherEntry");

                saveUdefEntry(item,tran);

                DBHelper.GetInstance(mContext).CommitTransaction(tran);

                return item.header.id;
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

        void saveUdefEntry(Voucher item, dynamic tran)
        {
            if (item.udefenties == null)
                return;
            var udefTemplate = TemplateSevice.GetInstance(mContext).FindUdefTemplate("_VoucherEntryUdef");
            DataTable dtUdef = new DataTable();
            dtUdef.Columns.Add(new DataColumn("_id", typeof(long)));
            dtUdef.Columns.Add(new DataColumn("_uniqueKey", typeof(string)));
            udefTemplate.ForEach(udef => {
                var dc = new DataColumn("_" + udef.name, CommonUtils.ConvertDataTypeFromStr(udef.dataType));
                dtUdef.Columns.Add(dc);
            });

            foreach (var udef in item.udefenties)
            {
                DataRow dr = dtUdef.NewRow();
                dr["_id"] = item.header.id;
                dr["_uniqueKey"] = udef.Key;

                Dictionary<string, object> udefValues = udef.Value;

                foreach (var field in udef.Value)
                {
                    var temp = udefTemplate.FirstOrDefault(t => t.name == field.Key);
                    if (temp != null)
                    {
                        
                    }
                }
                foreach (var temp in udefTemplate)
                {
                    Type t = CommonUtils.ConvertDataTypeFromStr(temp.dataType);
                    if (udefValues.ContainsKey(temp.name))
                    {                        
                        dr["_" + temp.name] = Convert.ChangeType(udefValues[temp.name], t);
                    }
                    else
                    {
                        dr["_" + temp.name] = t.IsValueType ? Activator.CreateInstance(t) : DBNull.Value;
                    }
                }
                dtUdef.Rows.Add(dr);
            }
            DBHelper.GetInstance(mContext).InsertTable(tran, dtUdef, "_VoucherEntryUdef");
        }
        /// <summary>
        /// 数据有效性检查
        /// 头部：
        ///     1、凭证字、凭证号不能为空
        ///     2、业务日期、日期、年度、期间不能为空
        ///     3、日期、年度、期间不能为已结账期间
        /// 分录：
        ///     1、科目、金额不能为0
        ///     2、借贷方要平衡        
        /// </summary>
        /// <param name="item"></param>
        void CheckData(Voucher item)
        {
            var header = item.header;
            DataCheckHelper.StringIsNullOrEmpty(header.word,"凭证字不能为空");
            //DataCheckHelper.NumberIsZero(header.no, "凭证号不能为0");
            DataCheckHelper.IsNull(header.businessDate, "业务日期不能为空");
            DataCheckHelper.IsNull(header.date, "日期不能为空");
            DataCheckHelper.NumberIsZero(header.year, "会计年度不能为0");
            DataCheckHelper.NumberIsZero(header.period, "会计期间不能为0");
            DataCheckHelper.DateInPeriod(header.date, header.year, header.period, "日期和会计年度期间不符");

            SystemProfileService systemProfileService = SystemProfileService.GetInstance(mContext);
            var currentYear = systemProfileService.GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentYear);
            var currentPeriod = systemProfileService.GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentPeriod);
            DateTime current = new DateTime(currentYear, currentPeriod, 1);
            if (current > header.date)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA,"不能录入已过账期间的凭证");

            if (item.entries ==null || item.entries.Count == 0)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "凭证不能没有分录");

            decimal totalAmount = 0M;
            foreach(var entry in item.entries)
            {
                if (entry.accountSubjectId == 0 && string.IsNullOrEmpty(entry.accountSubjectNo))
                    throw new FinanceException(FinanceResult.IMPERFECT_DATA, string.Format("第{0}行分录，科目不能为空", entry.index));

                DataCheckHelper.NumberIsZero(entry.amount, string.Format("第{0}行分录，金额不能为0", entry.index));
                totalAmount += (entry.direction * entry.amount);
            }
            if (totalAmount != 0M)
                throw new FinanceException(FinanceResult.AMMOUNT_IMBALANCE);

        }

        /// <summary>
        /// 对凭证的各个ID进行处理:内码、序号、凭证号
        /// 序号要求连续，出现重复，直接取新的填充
        /// 凭证号，在同一会计年度、期间、凭证字下唯一，出现重复，报错
        /// </summary>
        /// <param name="item">凭证主体</param>
        /// <param name="tran">事务对象</param>
        void GeneratorIds(VoucherHeader header, dynamic tran)
        {
            string exKey = string.Format("{0}_{1}_{2}", header.year, header.period, header.word);
            SerialNoService noGenerator = new SerialNoService(mContext, tran) { SerialKey = SerialNoKey.VoucherNo, Ex = exKey };
            //var bExistNo = DBHelper.GetInstance(mContext).Exist(
            //    string.Format("select 1 from _voucherheader where _year = {0} and _period={1} and _word ='{2}' and _no={3}",
            //    header.year, header.period, header.word, header.no));
            //if (bExistNo)
            //    throw new FinanceException(FinanceResult.RECORD_EXIST, string.Format("{0}年度第{1}期间{2}字序号{3}", header.year, header.period, header.word, header.no));

            SerialNoService idGenerator = new SerialNoService(mContext, tran) { SerialKey=SerialNoKey.System };            
            SerialNoService snGenerator = new SerialNoService(mContext, tran) { SerialKey = SerialNoKey.VoucherSn };
            header.id = idGenerator.GetIncrease();
            header.serialNo = snGenerator.GetIncrease();
            header.no = noGenerator.GetIncrease();
            while (DBHelper.GetInstance(mContext).Exist(string.Format("select 1 from _voucherheader where _id = {0}", header.id)))
            {
                header.id = idGenerator.GetIncrease();
            }
            while (DBHelper.GetInstance(mContext).Exist(string.Format("select 1 from _voucherheader where _serialNo={0}", header.serialNo)))
            {
                header.serialNo = snGenerator.GetIncrease();
            }
            while (DBHelper.GetInstance(mContext).Exist(string.Format("select 1 from _voucherheader where _year = {0} and _period={1} and _word ='{2}' and _no={3}",
                header.year, header.period, header.word, header.no)))
            {
                header.no = noGenerator.GetIncrease();
            }
        }
       
        public void Delete(long id)
        {
            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                //已审核不能删除
                var bExist = DBHelper.GetInstance(mContext).Exist(tran, "select * from _VoucherHeader where _status > 0 and _id = " + id);
                if (bExist)
                    throw new FinanceException(FinanceResult.INCORRECT_STATE);

                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from _VoucherEntryUdef where _id = " + id );
                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from _VoucherEntry where _id= " + id);
                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from _VoucherHeader where _id= " + id);

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

        public long Linked(long id, int linked)
        {
            string where = "";
            if (linked == 0)
            {
                return id;
            }
            else if (linked == 1)
            {
                if (id == 0)
                    where = " order by _id desc";
                else
                    where = " where _id < " + id + " order by _id desc ";
            }
            else if(linked == 2) //下一张
                where = " where _id > " + id + " order by _id  ";
            var obj = DBHelper.GetInstance(mContext).ExecuteScalar("select top 1 _id from _VoucherHeader " + where);
            if (obj != null)
            {
                return (long)obj;
            }
            return 0;
        }

        public void Check(long id)
        {
            Action<dynamic> act = (tran) =>
            {
                DBHelper.GetInstance(mContext).ExecuteSql(tran,string.Format("update _VoucherHeader set _checker={0},_checkTime=GETDATE() where _id={1}", mContext["UserId"], id));
            };
            ChangeStatus(id, VoucherStatus.Normal, VoucherStatus.Checked, act);
        }

        public void UnCheck(long id)
        {
            ChangeStatus(id, VoucherStatus.Checked, VoucherStatus.Normal, null);
        }

        public void Cancel(long id)
        {
            ChangeStatus(id, VoucherStatus.Checked, VoucherStatus.Canceled, null);
        }

        public void UnCancel(long id)
        {            
            ChangeStatus(id, VoucherStatus.Canceled, VoucherStatus.Checked, null);
        }

        public void Post(long id)
        {
            ChangeStatus(id, VoucherStatus.Checked, VoucherStatus.Posted, null);
        }

        public void UnPost(long id)
        {
            ChangeStatus(id, VoucherStatus.Posted, VoucherStatus.Checked, null);
        }

        /// <summary>
        /// 改变凭证状态
        /// </summary>
        /// <param name="srcStatus">原来的状态，如果不符合报错1007，如果为Invalid.就不检查</param>
        /// <param name="destStatus"></param>
        /// <param name="act">在更改状态 同事务执行</param>
        void ChangeStatus(long id,VoucherStatus srcStatus, VoucherStatus destStatus,Action<dynamic> act)
        {
            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                if (srcStatus != VoucherStatus.Invalid)
                {                    
                    var obj = DBHelper.GetInstance(mContext).ExecuteScalar(tran, string.Format("select _status from _VoucherHeader where _id = {0}", id));
                    if((int)obj != (int)srcStatus)
                        throw new FinanceException(FinanceResult.INCORRECT_STATE);
                }

                DBHelper.GetInstance(mContext).ExecuteSql(tran, string.Format("update _VoucherHeader set _status={0} where _id={1} ", (int)destStatus, id));
                act?.Invoke(tran);

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


        public VoucherHeader FindHeader(long id)
        {
            if (id == 0)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA);
            return DataManager.GetInstance(mContext).Find<VoucherHeader>(id);
           
        }

        public List<VoucherEntry> FindEntrys(long id)
        {
            VoucherEntry entity = new VoucherEntry { id = id };
            return DataManager.GetInstance(mContext).Query(entity);
        }


        public Dictionary<string,Dictionary<string,object>> FindUdefEntrys(long id)
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt("select * from _VoucherEntryUdef where _id =" + id );
            if(dt == null)
                return result;
            foreach (DataRow dr in dt.Rows)
            {
                var map = EntityConvertor<int>.ToMap(dr);
                var key = dr["_uniqueKey"].ToString();
                result.Add(key, map);
            }
            return result;
        }

        public List<Voucher> List(IDictionary<string,object> filter)
        {
            var result= new List<Voucher>();
            string where = string.Empty;
            if (filter != null)
            {
                where = appendFilter(where, buildDateFilter(filter));
                where = appendFilter(where, buildStatusFilter(filter));
                where = appendFilter(where, buildUserFilter(filter));
                where = appendFilter(where, buildTextFilter(filter));

                if (where != "")
                    where = " where " + where;

                DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt("select _id from _VoucherHeader " + where);
                if (dt == null || dt.Rows.Count == 0)
                    return result;

                List<long> Ids1 = GetIdsFromDataTable(dt);

                var idFilter = string.Join(",", Ids1);
                where = "where _id in (" + idFilter + ")";
                where = appendFilter(where, buildEntryFilter(filter));
                DataTable dtIds = DBHelper.GetInstance(mContext).ExecuteDt("select _id from _VoucherEntry " + where);
                if (dtIds == null || dtIds.Rows.Count == 0)
                    return result;

                List<long> ids2 = GetIdsFromDataTable(dtIds);
                idFilter = string.Join(",", ids2);
                where = "where _id in (" + idFilter + ")";
            }
            DataTable dtHeader = DBHelper.GetInstance(mContext).ExecuteDt("select * from _VoucherHeader " + where);
            DataTable dtEntrys = DBHelper.GetInstance(mContext).ExecuteDt("select * from _VoucherEntry " + where);
            List<VoucherHeader> lstHeader = EntityConvertor<VoucherHeader>.ToList(dtHeader);
            List<VoucherEntry> lstEntreis = EntityConvertor<VoucherEntry>.ToList(dtEntrys);

            foreach (var header in lstHeader)
            {
                Voucher voucher = new Voucher();
                voucher.header = header;
                voucher.entries = lstEntreis.FindAll(entry=>entry.id== header.id);
                result.Add(voucher);
            }
            return result;
        }

        List<long> GetIdsFromDataTable(DataTable dt)
        {
            List<long> result = new List<long>();
            if (dt == null || dt.Rows.Count == 0)
                return result;
            if (!dt.Columns.Contains("_id"))
                return result;
            foreach (DataRow dr in dt.Rows)
            {
                long id = 0;
                var idStr = dr["_id"].ToString();
                if (long.TryParse(idStr, out id))
                {
                    if(!result.Contains(id))
                        result.Add(id);
                }
            }
            return result;
        }

        string appendFilter(string where,string append)
        {
            if (append == "")
                return where;
            if (where == "")
                return append;

            return where += " and " + append;
        }

        string buildDateFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("yearBegin") && filter.ContainsKey("periodBegin"))
            {
                int year = 0, period = 0;
                int.TryParse(filter["yearBegin"].ToString(),out year);
                int.TryParse(filter["periodBegin"].ToString(), out period);
                if (year > 0 && period > 0)
                {
                    var tmp = string.Format(" _date >= '{0}'", new DateTime(year,period,1).ToString("yyyy-MM-dd"));
                    if (result == "")
                        result = tmp;
                    else
                        result += " and " + tmp;
                }
            }

            if (filter.ContainsKey("yearEnd") && filter.ContainsKey("periodEnd"))
            {
                int year = 0, period = 0;
                int.TryParse(filter["yearEnd"].ToString(), out year);
                int.TryParse(filter["periodEnd"].ToString(), out period);
                if (year > 0 && period > 0)
                {
                    var tmp = string.Format(" _date <= '{0}'", new DateTime(year, period, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
                    if (result == "")
                        result = tmp;
                    else
                        result += " and " + tmp;
                }
            }

            if (filter.ContainsKey("dateBeign"))
            {
                DateTime date = new DateTime();
                var b = DateTime.TryParse(filter["dateBeign"].ToString(),out date);
                if (b)
                {
                    var tmp = string.Format(" _date >= '{0}'", date.ToString("yyyy-MM-dd"));
                    if (result == "")
                        result = tmp;
                    else
                        result += " and " + tmp;
                }
            }

            if (filter.ContainsKey("dateEnd"))
            {
                DateTime date = new DateTime();
                var b = DateTime.TryParse(filter["dateEnd"].ToString(), out date);
                if (b)
                {
                    var tmp = string.Format(" _date <= '{0}'", date.ToString("yyyy-MM-dd"));
                    if (result == "")
                        result = tmp;
                    else
                        result += " and " + tmp;
                }
            }

            return result;
        }

        string buildStatusFilter(IDictionary<string, object> filter)
        {
            bool bNormal=false,bChecked = false, bCanceled = false, bPosted = false, bSettled = false;
            if (filter.ContainsKey("normal"))
            {
                int result = 0;
                int.TryParse(filter["normal"].ToString(), out result);
                bNormal = result == 1;
            }
            if (filter.ContainsKey("checked"))
            {
                int result = 0;
                int.TryParse(filter["checked"].ToString(), out result);
                bChecked = result == 1;
            }
            if (filter.ContainsKey("canceled"))
            {
                int result = 0;
                int.TryParse(filter["canceled"].ToString(), out result);
                bCanceled = result == 1;
            }
            if (filter.ContainsKey("posted"))
            {
                int result = 0;
                int.TryParse(filter["posted"].ToString(), out result);
                bPosted = result == 1;
            }
            if (filter.ContainsKey("settled"))
            {
                int result = 0;
                int.TryParse(filter["settled"].ToString(), out result);
                bSettled = result == 1;
            }
            var str = string.Empty;

            if (bNormal)
            {
                var tmp = "_status = " + (int)VoucherStatus.Normal;
                if (str == "")
                    str = tmp;
                else
                    str += " or " + tmp;
            }
            if (bChecked)
            {
                var tmp = "_status = " + (int)VoucherStatus.Checked;
                if (str == "")
                    str = tmp;
                else
                    str += " or " + tmp;
            }
            if (bCanceled)
            {
                var tmp = "_status = " + (int)VoucherStatus.Canceled;
                if (str == "")
                    str = tmp;
                else
                    str += " or " + tmp;
            }
            if (bPosted)
            {
                var tmp = "_status = " + (int)VoucherStatus.Posted;
                if (str == "")
                    str = tmp;
                else
                    str += " or " + tmp;
            }
            if (bSettled)
            {
                var tmp = "_status = " + (int)VoucherStatus.Settled;
                if (str == "")
                    str = tmp;
                else
                    str += " or " + tmp;
            }
            if(str!="")
                str = "(" + str + ")";
            return str;
        }

        string buildUserFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("creater") && filter["creater"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["creater"].ToString(), out tmp);
                if (tmp > 0)
                {
                    if (result == "")
                        result = "_creater = " + tmp;
                    else
                        result += " and _creater = " + tmp;
                }
            }
            if (filter.ContainsKey("checker") && filter["checker"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["checker"].ToString(), out tmp);
                if (tmp > 0)
                {
                    if (result == "")
                        result = "_checker = " + tmp;
                    else
                        result += " and _checker = " + tmp;
                }
            }
            if (filter.ContainsKey("poster") && filter["poster"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["poster"].ToString(), out tmp);
                if (tmp > 0)
                {
                    if (result == "")
                        result = "_poster = " + tmp;
                    else
                        result += " and _poster = " + tmp;
                }
            }
            return result;
        }

        string buildTextFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("cashier") && filter["cashier"] != null)
            {
                var tmp = filter["cashier"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (result == "")
                        result = "_cashier = '" + tmp + "'";
                    else
                        result += " and _cashier= '" + tmp + "'";
                }
            }
            if (filter.ContainsKey("agent") && filter["agent"] != null)
            {
                var tmp = filter["agent"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (result == "")
                        result = "_agent = '" + tmp + "'";
                    else
                        result += " and _agent= '" + tmp + "'";
                }
            }
            if (filter.ContainsKey("reference") && filter["reference"] != null)
            {
                var tmp = filter["reference"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (result == "")
                        result = "_reference = '" + tmp + "'";
                    else
                        result += " and _reference= '" + tmp + "'";
                }
            }
            if (filter.ContainsKey("word") && filter["word"] != null)
            {
                var tmp = filter["word"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (result == "")
                        result = "_word = '" + tmp + "'";
                    else
                        result += " and _word= '" + tmp + "'";
                }
            }
            return result;
        }

        string buildEntryFilter(IDictionary<string, object> filter)
        {
            string result = string.Empty;
            if (filter.ContainsKey("explanation") && filter["explanation"] != null)
            {
                var tmp = filter["explanation"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (result == "")
                        result = "_explanation like '%" + tmp + "%'";
                    else
                        result += " and _explanation like '%" + tmp + "%'";
                }
            }

            if (filter.ContainsKey("accountSubjectId") && filter["accountSubjectId"] != null)
            {
                long tmp = 0;
                long.TryParse(filter["accountSubjectId"].ToString(), out tmp);
                if (tmp > 0)
                {
                    if (result == "")
                        result = "_accountSubjectId = " + tmp;
                    else
                        result += " and _accountSubjectId = " + tmp;
                }
            }

            if (filter.ContainsKey("linkNo") && filter["linkNo"] != null)
            {
                var tmp = filter["linkNo"].ToString();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (result == "")
                        result = "_linkNo = '" + tmp + "'";
                    else
                        result += " and _linkNo = '" + tmp + "'";
                }
            }
            return result;
        }
    }
}
