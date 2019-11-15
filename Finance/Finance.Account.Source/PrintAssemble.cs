using Finance.Account.Service;
using Finance.Utils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Finance.Account.Source
{
    public struct PrintTemplateInfo
    {
        public string name;
        public string procName;
        public long id;
    }

    public class PrintAssemble
    {
        PrintTemplateInfo mTemplateInfo;
        DataTable mDtHeader;
        DataTable mDtEntry;
        string mOutPutFileName;
        int mEntryStart = 0;
        int mEntryEnd = 0;
        int mPages = 0;
        int mCurrentPage = 0;

        private string mCachePath
        {
            get
            {
                string relativePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                return Path.GetFullPath(relativePath + ".Cache/");
            }
        }

        public PrintAssemble(PrintTemplateInfo templateInfo, VoucherService service)
        {
            mTemplateInfo = templateInfo;
            SqlParameter[] prams = new SqlParameter[1];
            prams[0] = new SqlParameter("id", mTemplateInfo.id);
            DataSet dataSet = service.RunDataSetProc(mTemplateInfo.procName, prams);
            if (dataSet.Tables.Count > 0)
                mDtHeader = dataSet.Tables[0];
            if (dataSet.Tables.Count > 1)
                mDtEntry = dataSet.Tables[1];
        }

        public string Package()
        {
            FileHelper.FileExpiry(mCachePath, "*.xlsx", 60 * 10);
            FileHelper.FileExpiry(mCachePath, "*.xls", 60 * 10);

            IWorkbook workbook = getWorkBook();
            ISheet sheet = workbook.GetSheetAt(0);
            int rowCount = EntryRange(workbook, sheet);
            if (rowCount <= 0)
                return "";

            int templateLastRowNum = sheet.LastRowNum;
            int insertRowIndex = templateLastRowNum + 2;
            mPages = (int)Math.Ceiling((decimal)mDtEntry.Rows.Count / rowCount);
            for (int i = 1; i < mPages; ++i)
            {
                SheetClone.CopyRows(workbook, sheet, 0, templateLastRowNum, insertRowIndex);
                insertRowIndex += templateLastRowNum + 2;
            }

            var lstTemplate = getEntrysTemplate(sheet);
            IRow row;
            for (int p = 0; p < mPages; ++p)
            {
                mCurrentPage = p + 1;
                if (p > 0)
                {
                    mEntryStart += templateLastRowNum + 2;
                    mEntryEnd += templateLastRowNum + 2;
                }
                for (int i = (templateLastRowNum + 2) * p; i <= templateLastRowNum * (p + 1) + 2 * p; i++)
                {
                    row = sheet.GetRow(i);
                    if (row == null)
                        continue;

                    var dataIndex = i - mEntryStart + rowCount * p;
                    if (i >= mEntryStart && i < mEntryEnd)
                        ReplaceEntryRow(row, dataIndex, lstTemplate);
                    else
                        ReplaceRow(row);
                }
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);            
            using (FileStream fs = new FileStream(mOutPutFileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();

            GC.Collect();//强行销毁
            return mOutPutFileName;
        }




        List<string> getEntrysTemplate(ISheet sheet)
        {
            List<string> lst = new List<string>();
            IRow row = sheet.GetRow(mEntryStart);
            if (row != null)
            {
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    ICell cell = row.Cells[j];
                    if (cell == null)
                        lst.Add("");
                    else
                        lst.Add(cell.ToString());
                }
            }
            return lst;
        }

        public int EntryRange(IWorkbook workbook, ISheet sheet)
        {
            mEntryStart = 0;
            mEntryEnd = 0;
            IRow row;
            bool bStart = false;
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                    continue;
                if (row.Cells.Count == 0)
                    continue;
                ICell cell = row.Cells[0];
                if (cell == null)
                    continue;
                string cellValue = cell.ToString();
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    List<string> lst = Matchs(cellValue);
                    var str = lst.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        string regx = str.TrimStart('$', '{').TrimEnd('}');
                        string[] p = regx.Split('.');
                        if (p[0] == "Entry")
                        {
                            mEntryStart = i;
                            bStart = true;
                        }
                        else if (bStart)
                        {
                            mEntryEnd = i;
                            break;
                        }
                    }
                }
                else if (bStart && (i - mEntryStart) == mDtEntry.Rows.Count)
                {
                    mEntryEnd = i;
                    break;
                }
            }
            return mEntryEnd - mEntryStart;
        }

        void ReplaceEntryRow(IRow row, int dataIndex, List<string> lstTemplate)
        {
            for (int j = 0; j < row.Cells.Count; j++)
            {
                ICell cell = row.Cells[j];
                if (cell == null)
                    continue;
                string cellValue = lstTemplate[j];
                Dictionary<string, string> dict = new Dictionary<string, string>();
                ReplaceCell(cellValue, dataIndex, ref dict);
                foreach (var kv in dict)
                {
                    cellValue = cellValue.Replace(kv.Key, kv.Value);
                }
                cell.SetCellValue(cellValue);
            }
        }

        void ReplaceRow(IRow row)
        {
            for (int j = 0; j < row.Cells.Count; j++)
            {
                ICell cell = row.Cells[j];
                if (cell == null)
                    continue;
                string cellValue = cell.ToString();
                Dictionary<string, string> dict = new Dictionary<string, string>();
                ReplaceCell(cellValue, 0, ref dict);
                foreach (var kv in dict)
                {
                    cellValue = cellValue.Replace(kv.Key, kv.Value);
                }
                cell.SetCellValue(cellValue);
            }
        }

        void ReplaceCell(string input, int dataIndex, ref Dictionary<string, string> dict)
        {
            List<string> lst = Matchs(input);
            foreach (string val in lst)
            {
                string regx = val.TrimStart('$', '{').TrimEnd('}');
                string[] p = regx.Split('.');
                string v = "";
                if (p[0] == "Header")
                {
                    v = mDtHeader.Rows[0][p[1]].ToString();
                }
                else if ((p[0] == "Entry") && (mDtEntry.Rows.Count > dataIndex))
                {
                    v = mDtEntry.Rows[dataIndex][p[1]].ToString();
                }
                else if (p[0] == "Template")
                {
                    if (p[1] == "Pages")
                    {
                        v = mCurrentPage + "/" + mPages;
                    }
                }
                dict[val] = v;
            }
        }

        List<string> Matchs(string input)
        {
            List<string> lst = new List<string>();
            int start = -1;
            for (int i = 0; i < input.Length; ++i)
            {
                if ((input[i] == '$') && (i + 1 < input.Length) && (input[i + 1] == '{'))
                {
                    start = i;
                    ++i;
                }

                if ((start > -1) && (input[i] == '}'))
                {
                    lst.Add(input.Substring(start, i - start + 1));
                    start = -1;
                }
            }

            return lst;
        }


        IWorkbook getWorkBook()
        {
            IWorkbook workbook = null;
            string relativePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string tempalateFileName = Path.GetFullPath(relativePath + "PrintTemplate/" + mTemplateInfo.name);
            string tmpFileName = CommonUtils.GetUUID();
            mOutPutFileName = mCachePath + tmpFileName + FileHelper.FileSuffix(tempalateFileName);
            if (!FileHelper.CopyFile(tempalateFileName, mOutPutFileName))
            {
                return workbook;
            }

            using (FileStream fs = File.OpenRead(mOutPutFileName))
            {
                //这里需要根据文件名格式判断一下
                //HSSF只能读取xls的
                //XSSF只能读取xlsx格式的
                if (Path.GetExtension(fs.Name) == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else if (Path.GetExtension(fs.Name) == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
            }
            return workbook;
        }
    }



    public class SheetClone
    {
        public static void CopyRows(IWorkbook wb, ISheet sheet, int fromRowIndex, int toRowIndex, int insertRowIndex)
        {
            IRow row;
            IRow srcRow;
            for (int j = fromRowIndex; j < toRowIndex + 1; j++)
            {
                row = sheet.CreateRow(insertRowIndex + j);
                srcRow = sheet.GetRow(j);
                if (srcRow != null)
                    CopyRow(wb, srcRow, row, true);
            }

            CellRangeAddress fromRangeAddr;
            int sheetMergerCount = sheet.NumMergedRegions;
            for (int i = 0; i < sheetMergerCount; i++)
            {
                fromRangeAddr = sheet.GetMergedRegion(i);
                if (fromRangeAddr.LastRow > toRowIndex)
                    continue;
                CellRangeAddress newRangeAddr = new CellRangeAddress(
                    fromRangeAddr.FirstRow + insertRowIndex,
                    fromRangeAddr.LastRow + insertRowIndex,
                    fromRangeAddr.FirstColumn,
                    fromRangeAddr.LastColumn);
                sheet.AddMergedRegion(newRangeAddr);
            }
        }


        public static void CopyCellStyle(IWorkbook wb, ICellStyle fromStyle, ICellStyle toStyle)
        {
            toStyle.Alignment = fromStyle.Alignment;
            //边框和边框颜色
            toStyle.BorderBottom = fromStyle.BorderBottom;
            toStyle.BorderLeft = fromStyle.BorderLeft;
            toStyle.BorderRight = fromStyle.BorderRight;
            toStyle.BorderTop = fromStyle.BorderTop;
            toStyle.TopBorderColor = fromStyle.TopBorderColor;
            toStyle.BottomBorderColor = fromStyle.BottomBorderColor;
            toStyle.RightBorderColor = fromStyle.RightBorderColor;
            toStyle.LeftBorderColor = fromStyle.LeftBorderColor;

            //背景和前景
            toStyle.FillBackgroundColor = fromStyle.FillBackgroundColor;
            toStyle.FillForegroundColor = fromStyle.FillForegroundColor;

            toStyle.DataFormat = fromStyle.DataFormat;
            toStyle.FillPattern = fromStyle.FillPattern;
            //toStyle.Hidden=fromStyle.Hidden;
            toStyle.IsHidden = fromStyle.IsHidden;
            toStyle.Indention = fromStyle.Indention;//首行缩进
            toStyle.IsLocked = fromStyle.IsLocked;
            toStyle.Rotation = fromStyle.Rotation;//旋转
            toStyle.VerticalAlignment = fromStyle.VerticalAlignment;
            toStyle.WrapText = fromStyle.WrapText;
            toStyle.SetFont(fromStyle.GetFont(wb));
        }

        /// <summary>
        /// 复制表
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="fromSheet"></param>
        /// <param name="toSheet"></param>
        /// <param name="copyValueFlag"></param>
        public static void CopySheet(IWorkbook wb, ISheet fromSheet, ISheet toSheet, bool copyValueFlag)
        {
            //合并区域处理
            MergerRegion(fromSheet, toSheet);
            System.Collections.IEnumerator rows = fromSheet.GetRowEnumerator();
            while (rows.MoveNext())
            {
                IRow row = null;
                if (wb is HSSFWorkbook)
                    row = rows.Current as HSSFRow;
                else
                    row = rows.Current as NPOI.XSSF.UserModel.XSSFRow;
                IRow newRow = toSheet.CreateRow(row.RowNum);
                CopyRow(wb, row, newRow, copyValueFlag);
            }
        }

        /// <summary>
        /// 复制行
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="fromRow"></param>
        /// <param name="toRow"></param>
        /// <param name="copyValueFlag"></param>
        public static void CopyRow(IWorkbook wb, IRow fromRow, IRow toRow, bool copyValueFlag)
        {
            System.Collections.IEnumerator cells = fromRow.GetEnumerator(); //.GetRowEnumerator();
            toRow.Height = fromRow.Height;
            while (cells.MoveNext())
            {
                ICell cell = null;
                //ICell cell = (wb is HSSFWorkbook) ? cells.Current as HSSFCell : cells.Current as NPOI.XSSF.UserModel.XSSFCell;
                if (wb is HSSFWorkbook)
                    cell = cells.Current as HSSFCell;
                else
                    cell = cells.Current as NPOI.XSSF.UserModel.XSSFCell;
                ICell newCell = toRow.CreateCell(cell.ColumnIndex);
                CopyCell(wb, cell, newCell, copyValueFlag);
            }
        }


        /// <summary>
        /// 复制原有sheet的合并单元格到新创建的sheet
        /// </summary>
        /// <param name="fromSheet"></param>
        /// <param name="toSheet"></param>
        public static void MergerRegion(ISheet fromSheet, ISheet toSheet)
        {
            int sheetMergerCount = fromSheet.NumMergedRegions;
            for (int i = 0; i < sheetMergerCount; i++)
            {
                //Region mergedRegionAt = fromSheet.GetMergedRegion(i); //.MergedRegionAt(i);
                //CellRangeAddress[] cra = new CellRangeAddress[1];
                //cra[0] = fromSheet.GetMergedRegion(i);
                //Region[] rg = Region.ConvertCellRangesToRegions(cra);
                toSheet.AddMergedRegion(fromSheet.GetMergedRegion(i));
            }
        }

        /// <summary>
        /// 复制单元格
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srcCell"></param>
        /// <param name="distCell"></param>
        /// <param name="copyValueFlag"></param>
        public static void CopyCell(IWorkbook wb, ICell srcCell, ICell distCell, bool copyValueFlag)
        {
            ICellStyle newstyle = wb.CreateCellStyle();
            CopyCellStyle(wb, srcCell.CellStyle, newstyle);

            //样式
            distCell.CellStyle = newstyle;
            //评论
            if (srcCell.CellComment != null)
            {
                distCell.CellComment = srcCell.CellComment;
            }
            // 不同数据类型处理
            CellType srcCellType = srcCell.CellType;
            distCell.SetCellType(srcCellType);
            if (copyValueFlag)
            {
                if (srcCellType == CellType.Numeric)
                {

                    if (HSSFDateUtil.IsCellDateFormatted(srcCell))
                    {
                        distCell.SetCellValue(srcCell.DateCellValue);
                    }
                    else
                    {
                        distCell.SetCellValue(srcCell.NumericCellValue);
                    }
                }
                else if (srcCellType == CellType.String)
                {
                    distCell.SetCellValue(srcCell.RichStringCellValue);
                }
                else if (srcCellType == CellType.Blank)
                {
                    // nothing21
                }
                else if (srcCellType == CellType.Boolean)
                {
                    distCell.SetCellValue(srcCell.BooleanCellValue);
                }
                else if (srcCellType == CellType.Error)
                {
                    distCell.SetCellErrorValue(srcCell.ErrorCellValue);
                }
                else if (srcCellType == CellType.Formula)
                {
                    distCell.SetCellFormula(srcCell.CellFormula);
                }
                else
                { // nothing29
                }
            }
        }
    }
}

