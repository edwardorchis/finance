using Finance.Utils;
using Finance.Account.SDK;
using System.Collections.Generic;
using System.Data;
using Finance.Account.Service;

namespace Finance.Account.Source.DTL
{
    public partial class AuxiliaryDTL : IImportHandler
    {

        ILogger logger = Logger.GetLogger(typeof(AuxiliaryDTL));

        long mTid = 0;
        public void SetTid(long tid)
        {
            mTid = tid;
        }
        private string _fileName = Generator.getSourcePath() + "BaseData\\辅助资料.xlsx";

        public void SetFileName(string fileName)
        {
            _fileName = fileName;
        }
        public string GetDTLFileName()
        {
            return _fileName;
        }

        void IImportHandler.ActionAfterCommit()
        {
            
        }

        void IImportHandler.ActionBeforeCommit(dynamic tran)
        {
            
        }

        void IImportHandler.Deconde(ref DataSet ds)
        {
            DataTable dtDetail = null;
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName == "辅助资料")
                {
                    dtDetail = dt;
                    break;
                }
            }

            if (dtDetail == null)
            {
                throw new FinanceException(FinanceResult.IMPERFECT_DATA);
            }

            int recordCount = dtDetail.Rows.Count;
            long firstId = SerialNoService.GetInstance(new Dictionary<string, object> { { "Tid", mTid} }).GetIncrease(recordCount,SerialNoKey.System);

            DataSet dsResult = new DataSet();            
            dsResult.Tables.Add(decondeAuxiliary(dtDetail, firstId));
            ds = dsResult;
        }

        DataTable decondeAuxiliary(DataTable dtDetail, long idIndex)
        {
            List<Auxiliary> lst = new List<Auxiliary>();
            foreach (DataRow dr in dtDetail.Rows)
            {
                Auxiliary at = new Auxiliary();
                at.id = idIndex;
                AuxiliaryType atype = AuxiliaryType.Invalid;
                foreach (DataColumn col in dtDetail.Columns)
                {                    
                    switch (col.ColumnName)
                    {
                        case "代码":
                            at.no = dr[col.ColumnName].ToString();
                            break;
                        case "名称":
                            at.name = dr[col.ColumnName].ToString();
                            break;
                        case "描述":
                            at.description = dr[col.ColumnName].ToString();
                            break;
                        case "类型":
                            string strType = dr[col.ColumnName].ToString();
                            atype = AuxiliaryTypeMap[strType];
                            at.type = (long)atype;
                            break;
                        case "上级代码":
                            string strParent = dr[col.ColumnName].ToString();
                            long pid = 0L;
                            if (atype == AuxiliaryType.AccountGroup)
                                at.parentId = (long)AccountClassMap[strParent];
                            else if(long.TryParse(strParent,out pid))
                                at.parentId = pid;
                            break;                      
                    }
                }
                if (!string.IsNullOrEmpty(at.no))
                {
                    lst.Add(at);
                    idIndex++;
                }
            }
            //logger.Info(JsonConverter.JsonSerialize(lst));
            DataTable dtRsp = EntityConvertor<Auxiliary>.ToDataTable(lst);
            dtRsp.TableName = "Auxiliary";
            return dtRsp;
        }
    }
}
