using Finance.Account.SDK;
using Finance.Account.Service;
using Finance.Utils;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Finance.Account.Source.DTL
{
    public class AccountSubjectDTL : IImportHandler
    {
        ILogger logger = Logger.GetLogger(typeof(AccountSubjectDTL));

        long mTid = -1;
        public void SetTid(long tid)
        {
            mTid = tid;
        }
        public string GetDTLFileName()
        {
            return Generator.getSourcePath() + "BaseData\\科目.xls";
        }

        void IImportHandler.ActionAfterCommit()
        {
           
        }

        void IImportHandler.ActionBeforeCommit(dynamic tran)
        {
           
        }

        void IImportHandler.Deconde(ref DataSet ds)
        {
            DataTable dt = ds.Tables[0];
            DataRow[] dataRows = dt.Select("1=1", "代码 asc");

            long idIndex = SerialNoService.GetInstance(new Dictionary<string, object> { { "Tid", mTid } }).GetIncrease(dt.Rows.Count, SerialNoKey.System);        

            Auxiliary auxiliary = new Auxiliary() { type = (long)AuxiliaryType.AccountGroup };
            var lstAuxiliary = DataManager.GetInstance(new Dictionary<string, object> { { "Tid", mTid} }).Query<Auxiliary>(auxiliary) ;

            ///TODO:
            ///     1、列名映射
            ///     2、值转换
            ///     3、计算父子关系           
            List<AccountSubject> lst = new List<AccountSubject>();
            foreach (DataRow dr in dataRows)
            {
                AccountSubject at = new AccountSubject();
                at.id = idIndex;
                foreach (DataColumn col in dt.Columns)
                {
                    switch (col.ColumnName)
                    {
                        case "代码":
                            var no = dr[col.ColumnName].ToString();
                            at.no = no;
                            var pointIndex = at.no.LastIndexOf(".");
                            if (pointIndex > -1)
                            {
                                var parentNo = no.Substring(0, pointIndex );
                                AccountSubject parentAccount = lst.FirstOrDefault(a=>a.no == parentNo);
                                if (parentAccount == null)
                                {
                                    throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, "找不到父节点科目【" + parentNo + "】.");
                                }
                                at.parentId = parentAccount.id;
                                at.level = parentAccount.level + 1;
                                at.rootId = parentAccount.rootId == 0 ? parentAccount.id : parentAccount.rootId;
                                parentAccount.isHasChild = true;
                            }
                            else
                            {
                                at.level = 1;
                            }
                            break;
                        case "名称":
                            at.name = dr[col.ColumnName].ToString();
                            at.fullName = CalcFullName(lst, at);
                            break;                     
                        case "类别":
                            string strAux = dr[col.ColumnName].ToString();
                            Auxiliary aux = lstAuxiliary.Find(t => t.name == strAux);
                            if (aux == null)
                            {
                                //如果需要，去数据库检索一遍
                                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, "类别【" + strAux + "】.");
                            }
                            at.groupId = aux.id;
                            break;
                        case "余额方向":
                            at.direction = dr[col.ColumnName].ToString() == "借" ? 1 : -1;
                            break;
                        //case "现金科目":
                        //    at.isCashSubject = dr[col.ColumnName].ToString() == "是";
                        //    break;
                        //case "银行科目":
                        //    at.isBankSubject = dr[col.ColumnName].ToString() == "是";
                        //    break;
                        //case "现金等价物":
                        //    at.isCashEqulvalent = dr[col.ColumnName].ToString() == "是";
                        //    break;
                        //case "主表项目":
                        //    var val = dr[col.ColumnName].ToString();
                        //    if(!string.IsNullOrEmpty(val))
                        //        at.mainProjectId = 0L;
                        //    break;
                    }                    
                }
                if (!string.IsNullOrEmpty(at.no))
                {
                    lst.Add(at);
                    idIndex++;
                }
            }
            DataTable dtRsp = EntityConvertor<AccountSubject>.ToDataTable(lst);
            dtRsp.TableName = "AccountSubject";
            ds = new DataSet();
            ds.Tables.Add(dtRsp);           
        }

        string CalcFullName(List<AccountSubject> lst, AccountSubject at)
        {
            var result = at.name;
            while (at.parentId != 0)
            {
                at = lst.FirstOrDefault(s=>s.id == at.parentId);
                if (at == null)
                    break;
                result = at.name + "/" + result;
            }
            return result;
        }
    }
}
