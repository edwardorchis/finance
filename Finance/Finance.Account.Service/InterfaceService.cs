using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public class InterfaceService
    {
        static ILogger logger = Logger.GetLogger(typeof(InterfaceService));
        DBHelper mBDBHelper = null;
        private IDictionary<string, object> mContext;
        public InterfaceService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
            mBDBHelper = SystemProfileService.GetInstance(ctx).GetBDBHelper();
        }

        public static InterfaceService GetInstance(IDictionary<string, object> ctx)
        {
            return new InterfaceService(ctx);
        }

        public string ExecTask(ExecTaskType taskType, string procName, Dictionary<string, object> filter)
        {
            var taskId = SerialNoService.GetUUID();
            try
            {
                switch (taskType)
                {
                    case ExecTaskType.CreateVoucher:
                        ExecCreateVoucher(taskId, procName, filter);
                        break;
                    case ExecTaskType.CarriedForward:
                        ExecCarriedForward(taskId, procName, filter);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                RefreshTaskResult(taskId, ExecTaskType.CreateVoucher.ToString(), -1, "", ex.Message);
                //throw new FinanceException(FinanceResult.SYSTEM_ERROR);
            }

            return taskId;
        }

        void ExecCreateVoucher(string taskId, string procName, Dictionary<string, object> filter)
        {
            DataSet ds = null;
            if (filter == null)
                ds = (DataSet)mBDBHelper.RunDataSetProc(procName);
            else
            {
                var prams = new SqlParameter[filter.Keys.Count];
                var i = 0;
                foreach (var kv in filter)
                {
                    prams[i++] = new SqlParameter(kv.Key, kv.Value);
                }
                ds = (DataSet)mBDBHelper.RunDataSetProc(procName, prams);
            }

            var lstHeader = EntityConvertor<VoucherHeader>.ToList(ds.Tables[0]);
            var lstEntries = EntityConvertor<VoucherEntry>.ToList(ds.Tables[1]);

            var dtUdefenties = ds.Tables[2];
            var lstUdefenties = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            foreach (DataRow dr in dtUdefenties.Rows)
            {
                var map = EntityConvertor<Object>.ToMap(dr);
                var linkNo = map["linkNo"].ToString();
                if (lstUdefenties.ContainsKey(linkNo))
                {
                    lstUdefenties[linkNo].Add(map["uniqueKey"].ToString(), map);
                }
                else
                {
                    var val = new Dictionary<string, Dictionary<string, object>>();
                    val.Add(map["uniqueKey"].ToString(), map);
                    lstUdefenties.Add(linkNo, val);
                }
            }
           
            RefreshTaskResult(taskId, ExecTaskType.CreateVoucher.ToString(), 0, "开始执行", "");

            //这里需要异步
            var lstAccountSubjectNo = lstEntries.Select(e => e.accountSubjectNo).Distinct().ToList();
            var bRnt = GenerateAccoutSubject(taskId, lstAccountSubjectNo);
            if (bRnt)
            {
                GenerateVoucher(taskId, lstHeader, lstEntries, lstUdefenties);
            }
        }

        bool GenerateAccoutSubject(string taskId,List<string> lstAccountSubjectNo)
        {
            var sb = new StringBuilder();
            var lstAccountSubject = AccountSubjectService.GetInstance(mContext).List();
            var lstExists = lstAccountSubject.Select(a => a.no).ToList();
            var lstNotExist = new List<string>();
            foreach(var aso in lstAccountSubjectNo)
            {
                if (!(lstExists.Exists(a => aso == a)))
                {
                    if (!aso.Contains("."))
                    {
                        sb.AppendLine(string.Format("不能生成一级科目[{0}];", aso));
                        continue;
                    }

                    var parentNo = aso.Substring(0, aso.LastIndexOf("."));
                    if (!lstExists.Exists(a => a == parentNo))
                    {
                        sb.AppendLine(string.Format("科目[{0}]的上级科目[{1}]不存在，请在科目中新增后重试;", aso, parentNo));
                        continue;
                    }

                    var prams = new SqlParameter[] { new SqlParameter("accountSubjectNo", aso) };
                    var asoName = mBDBHelper.RunProcScalar("sp_getaccountsubjectname", prams);
                    if (asoName == null || string.IsNullOrEmpty(asoName.ToString()))
                    {
                        sb.AppendLine(string.Format("自动创建科目[{0}]获取科目名称失败;", aso));
                        continue;
                    }

                    var parentAso = lstAccountSubject.FirstOrDefault(a=> a.no == parentNo);
                    var newAso = new AccountSubject();
                    newAso.level = parentAso.level + 1;
                    newAso.name = asoName.ToString();
                    newAso.no = aso;
                    newAso.parentId = parentAso.id;
                    newAso.rootId = parentAso.rootId == 0? parentAso.id: parentAso.rootId;
                    newAso.groupId = parentAso.groupId;
                    newAso.direction = parentAso.direction;
                    AccountSubjectService.GetInstance(mContext).Save(newAso);
                    sb.AppendLine(string.Format("自动创建科目[{0} - {1}]成功;", aso, asoName.ToString()));
                }
            }
            RefreshTaskResult(taskId, ExecTaskType.CreateVoucher.ToString(), 20, "", sb.ToString());
            return true;
        }

        void GenerateVoucher(string taskId,List<VoucherHeader> lstHeader,List<VoucherEntry> lstEntries, Dictionary<string, Dictionary<string, Dictionary<string, object>>> lstUdefenties)
        {
            var success = true;
            var sb = new StringBuilder();
            foreach (var header in lstHeader)
            {
                if (!lstEntries.Exists(e => e.linkNo == header.linkNo))
                {
                    sb.AppendLine(string.Format("关联单号[{0}]生成凭证失败，未找到关联的分录;",header.linkNo));
                    success = false;
                    continue;
                }

                var voucher = new Voucher();
                voucher.header = header;
                voucher.entries = lstEntries.FindAll(e => e.linkNo == header.linkNo);
                if (lstUdefenties.ContainsKey(header.linkNo))
                {
                    voucher.udefenties = lstUdefenties[header.linkNo];
                }
                try
                {
                    var id = VoucherService.GetInstance(mContext).Add(voucher);
                    var h = VoucherService.GetInstance(mContext).FindHeader(id);
                    sb.AppendLine(string.Format("关联单号[{0}]生成凭证成功，凭证字号：{1} - {2};", header.linkNo, h.word, h.no));
                }
                catch (Exception ex)
                {
                    sb.AppendLine(string.Format("关联单号[{0}]生成凭证失败，{1};", header.linkNo, ex.Message));
                    success = false;
                }
            }
            RefreshTaskResult(taskId, ExecTaskType.CreateVoucher.ToString(), 100, "", sb.ToString(), success?1:0);
        }
        
        public void RefreshTaskResult(string taskId, string taskType, int progRate, string reserved, string result, int status = 0)
        {            
            if (DBHelper.GetInstance(mContext).Exist(string.Format("select 1 from _TaskResult where _taskId = '{0}'", taskId)))
            {
                var sql = string.Format("update _TaskResult set _progRate = '{0}',_reserved = '{1}',_result = _result + '{2}',_status = {3} where _taskId = '{4}'", progRate,reserved, result, status, taskId);
                DBHelper.GetInstance(mContext).ExecuteSql(sql);
            }
            else
            {
                var taskResult = new TaskResult();
                taskResult.taskId = taskId;
                taskResult.taskType = taskType;
                taskResult.createTime = DateTime.Now;
                taskResult.lastRefreshTime = DateTime.Now;
                taskResult.progRate = progRate;
                taskResult.reserved = reserved;
                taskResult.result = result;
                DataManager.GetInstance(mContext).Insert<TaskResult>(taskResult);
            }
           
        }
        
        public TaskResult GetResult(string taskId)
        {
            TaskResult taskResult = new TaskResult { taskId = taskId };
            var lst = DataManager.GetInstance(mContext).Query(taskResult);
            if (lst == null || lst.Count == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            return lst.FirstOrDefault();
        }

        void ExecCarriedForward(string taskId, string procName, Dictionary<string, object> paramMap)
        {
            if (paramMap == null)
                paramMap = new Dictionary<string, object>();

            RefreshTaskResult(taskId, ExecTaskType.CarriedForward.ToString(), 0, "开始执行", "");

            Auxiliary auxFilter = new Auxiliary() { type = (int)AuxiliaryType.CarriedForward, no = procName };
            var lst = DataManager.GetInstance(mContext).Query(auxFilter);
            if (lst == null || lst.Count == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, "结转方式不存在");

            var auxObj = lst.FirstOrDefault();
            var templateList = TemplateSevice.GetInstance(mContext).ListCarriedForwardTemplate(auxObj.id);
            if(templateList == null || templateList.Count == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, "结转模板不存在");
                    
            var year = SystemProfileService.GetInstance(mContext).GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentYear);
            var period = SystemProfileService.GetInstance(mContext).GetInt(SystemProfileCategory.Account, SystemProfileKey.CurrentPeriod);
            var dbHelper = DBHelper.GetInstance(mContext);
         
            //1、检查是否还有未过账
            if (dbHelper.Exist(string.Format("select 1 from _VoucherHeader where _year = {0} and _period = {1} and _status < {2} and _status <> {3}"
                , year,period,(int)VoucherStatus.Posted, (int)VoucherStatus.Canceled)))
                throw new FinanceException(FinanceResult.INCORRECT_STATE, "当前凭证没有全部过账");
            //2、生成凭证
            var direction = 1;
            switch (auxObj.description)
            {
                case "income":
                    
                    break;
                case "cost":
                    direction = -1;
                    break;
                case "investment":

                    break;
                case "profits":

                    break;
            }
            var word = "转";
            if (paramMap.ContainsKey("word"))
            {
                word = paramMap["word"].ToString();
            }
            var explanation = auxObj.name;
            if (paramMap.ContainsKey("explanation"))
            {
                explanation = paramMap["explanation"].ToString();
            }
            var lstAccountObject = AccountSubjectService.GetInstance(mContext).List();
            var lstEntries = new List<VoucherEntry>();
            var index = 0;
            foreach(var temp in templateList)
            {
                var srcActObj = lstAccountObject.FirstOrDefault(actObj => actObj.id == temp.src);
                var dstActObj = lstAccountObject.FirstOrDefault(actObj => actObj.id == temp.dst);
                if (srcActObj == null || dstActObj == null)
                    throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, "结转模板数据错误");

                var amountObj = dbHelper.ExecuteScalar(string.Format(@"select sum(_amount * _direction) as _amount 
from _VoucherEntry where _accountSubjectId = {0}
and _id in (select _id from _VoucherHeader where _year = {1} and _period = {2})", temp.src, year, period));
                if (amountObj == null || amountObj == DBNull.Value)
                    continue;

                var amount = (decimal)amountObj;
                if (amount < 0)
                {
                    direction = -1 * direction;
                    amount = -1 * amount;
                }
                
                var voucherEntrySrc = new VoucherEntry();
                voucherEntrySrc.index = index++;
                voucherEntrySrc.accountSubjectId = temp.src;
                voucherEntrySrc.accountSubjectNo = srcActObj.no;
                voucherEntrySrc.amount = amount;
                voucherEntrySrc.direction = direction;
                voucherEntrySrc.explanation = explanation;
                lstEntries.Add(voucherEntrySrc);

                var voucherEntryDst = new VoucherEntry();
                voucherEntryDst.index = index++;
                voucherEntryDst.accountSubjectId = temp.dst;
                voucherEntryDst.accountSubjectNo = dstActObj.no;
                voucherEntryDst.amount = amount;
                voucherEntryDst.direction = -1 * direction;
                voucherEntryDst.explanation = explanation;
                lstEntries.Add(voucherEntryDst);
            }
            if (lstEntries.Count == 0)
            {
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, "没有需要结转的凭证");
            }

            var voucherHeader = new VoucherHeader();
          
            voucherHeader.word = word;
            voucherHeader.year = year;
            voucherHeader.period = period;
            voucherHeader.date = CommonUtils.CalcMaxPeriodDate(year, period);
            var now = DateTime.Now;
            voucherHeader.businessDate = now;
            voucherHeader.creatTime = now;
            voucherHeader.creater = 13594;

            Voucher voucher = new Voucher();
            voucher.header = voucherHeader;
            voucher.entries = lstEntries;
            var id = VoucherService.GetInstance(mContext).Add(voucher);
            var h = VoucherService.GetInstance(mContext).FindHeader(id);
            var msg = string.Format("结转成功，凭证字号：{0} - {1};", h.word, h.no);
            RefreshTaskResult(taskId, ExecTaskType.CreateVoucher.ToString(), 100, "", msg, 1);
        }

    }


}
