using Finance.Utils;
using System.Collections.Generic;
using System.Data;
using Finance.Account.Source.Struct;
using Finance.Account.SDK;
using System;

namespace Finance.Account.Source.DTL
{
    public partial class BalanceSheetDTL : IImportHandler
    {

        ILogger logger = Logger.GetLogger(typeof(BalanceSheetDTL));

        public string GetDTLFileName()
        {
            return Generator.getSourcePath() + "BaseData\\资产负债表.xlsx";
        }
        long mTid = -1;
        public void SetTid(long tid)
        {
            mTid = tid;
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
                if (dt.TableName == "资产负债表")
                {
                    dtDetail = dt;
                    break;
                }
            }
            DataColumn col = new DataColumn("_name", typeof(string));
            dtDetail.Columns.Add(col);
            dtDetail.Columns["_name"].SetOrdinal(0);            
            dtDetail.Columns[1].ColumnName = "_a";
            dtDetail.Columns[2].ColumnName = "_b";
            dtDetail.Columns[3].ColumnName = "_c";
            dtDetail.Columns[4].ColumnName = "_d";
            dtDetail.Columns[5].ColumnName = "_e";
            dtDetail.Columns[6].ColumnName = "_f";
            dtDetail.Columns[7].ColumnName = "_g";
            dtDetail.Columns[8].ColumnName = "_h";

            foreach (DataRow dr in dtDetail.Rows)
            {
                dr[0] = "资产负债表";
            }
            dtDetail.TableName = "ExcelTemplate";            
        }     
    }
}
