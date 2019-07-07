using Finance.Utils;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public class ExcelImportor
    {
        private IImportHandler m_DTL = null;
        long mTid = -1;
        public ExcelImportor(long tid, IImportHandler dtl)
        {
            mTid = tid;
            dtl.SetTid(tid);
            m_DTL = dtl;         
        }
        private ILogger m_logger = Logger.GetLogger(typeof(ExcelImportor));


        public void Import()
        {
            if (m_DTL == null)
            {
                throw new Finance.Utils.FinanceException(FinanceResult.NULL_DTL);
            }            
            var file = m_DTL.GetDTLFileName();
            if (!File.Exists(file))
            {
                throw new Finance.Utils.FinanceException(FinanceResult.FILE_NOT_EXISST, file);
            }
            
            int startIndex = file.LastIndexOf("\\");
            int endIndex = file.LastIndexOf(".");
            var ds = ReadExcel(file);
            m_DTL.Deconde(ref ds);
            var db = DBHelper.GetInstance(new Dictionary<string, object> { { "Tid", mTid } });
            dynamic tran = db.BeginTransaction();
            try
            {
                foreach (DataTable dt in ds.Tables)
                {
                    var tableName ="_" + dt.TableName;
                    if (db.Exist(string.Format("select 1 from sysobjects where [type] = 'u' and name = '{0}'",tableName)))
                    {
                        db.InsertTable(tran, dt, tableName);
                    }
                    else
                    {
                        m_logger.Debug("don't exist table named " + tableName);
                    }
                }
                m_DTL.ActionBeforeCommit(tran);
                db.CommitTransaction(tran);
                m_DTL.ActionAfterCommit();
            }
            catch (Exception ex)
            {
                m_logger.Error(ex);
                db.RollbackTransaction(tran);
                throw new Finance.Utils.FinanceException(FinanceResult.SYSTEM_ERROR);
            }
         
        }

        DataSet ReadExcel(string fullfileName)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            FileStream fs = new FileStream(fullfileName, FileMode.Open, FileAccess.Read);
            //NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
            IWorkbook book = NPOI.SS.UserModel.WorkbookFactory.Create(fs);
            int sheetCount = book.NumberOfSheets;
            for (int sheetIndex = 0; sheetIndex < sheetCount; sheetIndex++)
            {
                NPOI.SS.UserModel.ISheet sheet = book.GetSheetAt(sheetIndex);
                if (sheet == null) continue;

                NPOI.SS.UserModel.IRow row = sheet.GetRow(0);
                if (row == null) continue;

                int firstCellNum = row.FirstCellNum;
                int lastCellNum = row.LastCellNum;
                if (firstCellNum == lastCellNum) continue;

                dt = new DataTable(sheet.SheetName);
                for (int i = firstCellNum; i < lastCellNum; i++)
                {
                    if (row.GetCell(i) == null) continue;
                    var colHeader = row.GetCell(i).StringCellValue;
                    int index = 1;
                    while (dt.Columns.Contains(colHeader))
                    {
                        colHeader += index;
                        index++;
                    }
                    dt.Columns.Add(colHeader, typeof(string));
                }

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow newRow = dt.Rows.Add();
                    for (int j = firstCellNum; j < lastCellNum; j++)
                    {
                        if (sheet.GetRow(i) == null) continue;
                        if (sheet.GetRow(i).GetCell(j) == null) continue;
                        ICell cell = sheet.GetRow(i).GetCell(j);
                        if (cell.CellType == CellType.String)
                            newRow[j] = cell.StringCellValue;
                        else if (cell.CellType == CellType.Numeric)
                            newRow[j] = cell.NumericCellValue;
                        else if (cell.CellType == CellType.Blank)
                            newRow[j] = DBNull.Value;
                        else
                            throw new Finance.Utils.FinanceException(FinanceResult.NOT_SUPPORT);
                    }
                }

                ds.Tables.Add(dt);
            }
            return ds;
        }

        
    }
}
