using Finance.Utils;
using System.Collections.Generic;
using System.Data;
using Finance.Account.Source.Struct;
using Finance.Account.SDK;
using System;

namespace Finance.Account.Source.DTL
{
    public partial class SystemProfileDTL : IImportHandler
    {

        ILogger logger = Logger.GetLogger(typeof(SystemProfileDTL));
        long mTid = -1;
        public void SetTid(long tid)
        {
            mTid = tid;
        }
        private string _fileName = Generator.getSourcePath() + "BaseData\\系统参数.xlsx";

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
                if (dt.TableName == "系统参数")
                {
                    dtDetail = dt;
                    break;
                }
            }

            dtDetail.Columns[0].ColumnName = "_category";
            dtDetail.Columns[1].ColumnName = "_key";
            dtDetail.Columns[2].ColumnName = "_value";
            dtDetail.Columns[3].ColumnName = "_description";
            dtDetail.TableName = "SystemProfile";
            
        }     
    }
}
