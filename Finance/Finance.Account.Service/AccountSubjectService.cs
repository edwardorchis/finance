using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finance.Account.Service
{
    public class AccountSubjectService
    {
        static ILogger logger = Logger.GetLogger(typeof(AccountSubjectService));
        private IDictionary<string, object> mContext;
        public AccountSubjectService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static AccountSubjectService GetInstance(IDictionary<string, object> ctx)
        {
            return new AccountSubjectService(ctx);
        }

        public List<AccountSubject> List(int status = 0)
        {
            string where = "";
            if (status != 0)
                where = " where _isDeleted = " + status;
            var dt = DBHelper.GetInstance(mContext).ExecuteDt("select * from _accountsubject " + where + " order by _no");

            return EntityConvertor<AccountSubject>.ToList(dt);
        }

        public AccountSubject Find(long id)
        {
            var aso = DataManager.GetInstance(mContext).Find<AccountSubject>(id);
            return aso;
        }

        public AccountSubject FindByNo(string no)
        {
            AccountSubject filter = new AccountSubject();
            filter.no = no;
            var lst = DataManager.GetInstance(mContext).Query(filter);
            if (lst.Count == 0)
                return null;
            return lst.First();
        }

        public void Save(AccountSubject aso)
        {
            if (string.IsNullOrEmpty(aso.no) || string.IsNullOrEmpty(aso.name))
            {
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "科目代码或名称不能为空" );
            }

            if (aso.direction != 1 && aso.direction != -1)
            {
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "科目余额方向无效");
            }

            var bRet = DBHelper.GetInstance(mContext).Exist(string.Format("select 1 from _Auxiliary where _type ={0} and _id = {1}", (int)AuxiliaryType.AccountGroup, aso.groupId));
            if (!bRet)
            {
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "科目类别无效");
            }

            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                bRet = DBHelper.GetInstance(mContext).Exist(tran, string.Format("select 1 from _accountsubject where _no ='{0}' and _id <> {1}", aso.no, aso.id));
                if (bRet)
                {
                    throw new FinanceException(FinanceResult.RECORD_EXIST, "代码已存在");
                }

                if (aso.id == 0)
                {
                    var pos = aso.no.LastIndexOf('.');
                    if (pos != -1)
                    {
                        var pre = aso.no.Substring(0, pos);
                        var filter = new AccountSubject
                        {
                            no = pre
                        };
                        List<AccountSubject> lst = DataManager.GetInstance(mContext).Query(tran, filter);
                        if (lst.Count == 0)
                        {
                            throw new FinanceException(FinanceResult.IMPERFECT_DATA, "父级科目不存在：" + pre);
                        }
                        var parentItem = lst.FirstOrDefault();
                        aso.parentId = parentItem.id;
                        aso.rootId = parentItem.rootId;
                        aso.level = parentItem.level + 1;

                        parentItem.isHasChild = true;
                        DataManager.GetInstance(mContext).Update(tran, parentItem);
                    }
                    SerialNoService serial = new SerialNoService(mContext, tran) { SerialKey = SerialNoKey.System };
                    aso.id = serial.GetIncrease();
                    aso.level = aso.level == 0 ? 1 : aso.level;
                    DataManager.GetInstance(mContext).Insert(tran, aso);
                }
                else
                {
                    DataManager.GetInstance(mContext).Update(tran, aso);
                }
                UpdateFullName(aso, tran);

                UserService.GetInstance(mContext).UpdateTimeStampArticle(TimeStampArticleEnum.AccountSubject, tran);
                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (FinanceException fex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                if (fex.HResult != (int)FinanceResult.RECORD_EXIST)
                    throw fex;
            }
            catch (Exception ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }
        }

        void UpdateFullName(AccountSubject aso, dynamic tran)
        {
            var dt = DBHelper.GetInstance(mContext).ExecuteDt(tran, "select * from _accountsubject order by _no");
            var lst = EntityConvertor<AccountSubject>.ToList(dt);            
            aso.fullName = CalcFullName(lst, aso);
            DBHelper.GetInstance(mContext).ExecuteSql(tran,string.Format("update _accountsubject set _fullName = '{0}' where _id = {1}", aso.fullName, aso.id));
        }

        string CalcFullName(List<AccountSubject> lst, AccountSubject at)
        {
            var result = at.name;
            while (at.parentId != 0)
            {
                at = lst.FirstOrDefault(s => s.id == at.parentId);
                if (at == null)
                    break;
                result = at.name + "/" + result;
            }
            return result;
        }


        public void Delete(long id)
        {
            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                var obj = DBHelper.GetInstance(mContext).ExecuteScalar(tran, "select 1 from _VoucherEntry where _accountSubjectId = " + id);
                if (obj != null)
                    throw new FinanceException(FinanceResult.LINKED_DATA);

                DBHelper.GetInstance(mContext).ExecuteSql(tran, "delete from  _accountsubject where _id = " + id);
                UserService.GetInstance(mContext).UpdateTimeStampArticle(TimeStampArticleEnum.AccountSubject, tran);
                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }
        }

        public void SetStatus(long id,int status)
        {
            dynamic tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                DBHelper.GetInstance(mContext).ExecuteSql(tran,"update _accountsubject set _isdeleted = " + status + " where _id = " + id);
                UserService.GetInstance(mContext).UpdateTimeStampArticle(TimeStampArticleEnum.AccountSubject,tran);
                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
                throw ex;
            }
        }
    }
}
