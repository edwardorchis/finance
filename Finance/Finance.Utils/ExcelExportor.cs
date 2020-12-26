using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Utils
{
    public class ExcelExportor
    {
        IExportHandler m_Handler = null;
        public ExcelExportor(IExportHandler handler)
        {
            m_Handler = handler;
        }

        public void Export(Stream ms,DataTable dt,string postfix)
        {
            //var fileName = m_Handler.GetFileName();
            NPOI.SS.UserModel.IWorkbook book = null;
            if (postfix == ".xls")
            {
                book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            }
            else if (postfix == ".xlsx")
            {
                book = new NPOI.XSSF.UserModel.XSSFWorkbook();
            }
            else
            {
                throw new FinanceException(FinanceResult.INCORRECT_STATE, "无效的文件名");
            }
            m_Handler.Encode(ref dt);

            //WriteExcel(ref book, dt);
            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("Sheet1");

            // 添加表头
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
            
            ICellStyle cellStyle = book.CreateCellStyle();
            var font = book.CreateFont();
            font.FontHeightInPoints = 14;
            font.IsBold = true;
            cellStyle.SetFont(font);
            int index = 0;
            //string caption = m_Handler.GetCaption();
            //if (string.IsNullOrEmpty(caption))
            //{
                foreach (DataColumn item in dt.Columns)
                {
                    NPOI.SS.UserModel.ICell cell = row.CreateCell(index);
                    cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    cell.CellStyle = cellStyle;
                    cell.SetCellValue(item.Caption);
                    index++;
                }
            //}
            //else
            //{
            //    NPOI.SS.UserModel.ICell cell = row.CreateCell(index);
            //    cell.SetCellType(NPOI.SS.UserModel.CellType.String);
            //    cell.CellStyle = cellStyle;
            //    cell.SetCellValue(caption);
            //}            

            // 添加数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                index = 0;
                row = sheet.CreateRow(i + 1);
                foreach (DataColumn item in dt.Columns)
                {
                    NPOI.SS.UserModel.ICell cell = row.CreateCell(index);
                    cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    cell.SetCellValue(dt.Rows[i][item].ToString());
                    index++;
                }
            }

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            //// 写入 
            //MemoryStream ms = new MemoryStream();
            book.Write(ms);
            book = null;

            //using (FileStream fs = new FileStream("E:\\Temp\\test.xls", FileMode.Create, FileAccess.Write))
            //{
            //    byte[] data = ms.ToArray();
            //    fs.Write(data, 0, data.Length);
            //    fs.Flush();
            //}

            //ms.Close();
            //ms.Dispose();
        }

        void WriteExcel(ref NPOI.SS.UserModel.IWorkbook book,DataTable dt)
        {
            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("Sheet1");

            // 添加表头
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
            int index = 0;
            foreach (DataColumn item in dt.Columns)
            {      
                NPOI.SS.UserModel.ICell cell = row.CreateCell(index);
                cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                cell.SetCellValue(item.Caption);
                index++;
            }

            // 添加数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                index = 0;
                row = sheet.CreateRow(i + 1);
                foreach (DataColumn item in dt.Columns)
                {
                    NPOI.SS.UserModel.ICell cell = row.CreateCell(index);
                    cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                    cell.SetCellValue(dt.Rows[i][item].ToString());
                    index++;
                }
            }           
        }
    }
}
