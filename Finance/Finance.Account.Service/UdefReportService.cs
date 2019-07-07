using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public class UdefReportService
    {
        static ILogger logger = Logger.GetLogger(typeof(UdefReportService));
        private IDictionary<string, object> mContext;
        public UdefReportService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static UdefReportService GetInstance(IDictionary<string, object> ctx)
        {
            return new UdefReportService(ctx);
        }

        public UdefReportDataSet Query(string procName, Dictionary<string, object> filter)
        {
            try
            {
                DataSet ds = null;
                if (filter == null)
                    ds = (DataSet)DBHelper.GetInstance(mContext).RunDataSetProc(procName);
                else
                {
                    var prams = new SqlParameter[filter.Keys.Count];
                    var i = 0;
                    foreach (var kv in filter)
                    {
                        prams[i++] = new SqlParameter(kv.Key, kv.Value);
                    }
                    ds = (DataSet)DBHelper.GetInstance(mContext).RunDataSetProc(procName, prams);
                }

                var dtHeader = ds.Tables[0];
                var dtEntries = ds.Tables[1];

                List<UdefTemplateItem> header = new List<UdefTemplateItem>();
                foreach (DataColumn dc in dtHeader.Columns)
                {
                    DataRow dr = dtHeader.Rows[0];
                    var item = new UdefTemplateItem
                    {
                        name = dc.ColumnName,
                        label = dr[dc.ColumnName].ToString()                        
                    };
                    
                    header.Add(item);
                }

                foreach (DataColumn dc in dtEntries.Columns)
                {                   
                    var item = header.FirstOrDefault(h=>h.name == dc.ColumnName);
                    if (item != null)
                    {
                        if (dc.DataType == typeof(long) || dc.DataType == typeof(decimal) || dc.DataType == typeof(byte)
                           || dc.DataType == typeof(sbyte) || dc.DataType == typeof(short) || dc.DataType == typeof(int)
                           || dc.DataType == typeof(ushort) || dc.DataType == typeof(uint) || dc.DataType == typeof(ulong)
                           || dc.DataType == typeof(float) || dc.DataType == typeof(double))
                        {
                            item.dataType = "number";
                        }
                    }                        
                }

                List<Dictionary<string, object>> entries = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dtEntries.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn dc in dtHeader.Columns)
                    {
                        dict.Add(dc.ColumnName, dr[dc.ColumnName]);
                    }
                    entries.Add(dict);
                }

                return new UdefReportDataSet { header = header,entries = entries};

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return null;
        }
    }
}
