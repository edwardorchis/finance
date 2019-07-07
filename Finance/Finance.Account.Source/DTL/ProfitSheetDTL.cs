using Finance.Utils;
using System.Collections.Generic;
using System.Data;
using Finance.Account.Source.Struct;
using Finance.Account.SDK;
using System;

namespace Finance.Account.Source.DTL
{
    public partial class ProfitSheetDTL : IImportHandler
    {

        ILogger logger = Logger.GetLogger(typeof(ProfitSheetDTL));
        long mTid = -1;
        public void SetTid(long tid)
        {
            mTid = tid;
        }
        public string GetDTLFileName()
        {
            return Generator.getSourcePath() + "BaseData\\利润表.xlsx";
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
                if (dt.TableName == "利润表")
                {
                    dtDetail = dt;
                }
            }
            DataColumn col = new DataColumn("_name", typeof(string));
            dtDetail.Columns.Add(col);
            dtDetail.Columns["_name"].SetOrdinal(0);            
            dtDetail.Columns[1].ColumnName = "_a";
            dtDetail.Columns[2].ColumnName = "_b";
            dtDetail.Columns[3].ColumnName = "_c";
            dtDetail.Columns[4].ColumnName = "_d";

            foreach (DataRow dr in dtDetail.Rows)
            {
                dr[0] = "利润表";
            }

            dtDetail.TableName = "ExcelTemplate";
        }     
    }
}
