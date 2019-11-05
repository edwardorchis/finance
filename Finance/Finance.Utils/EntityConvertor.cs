using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public static class EntityConvertor<T> where T : new()
    {
        public static T ToEntity(DataRow dr)
        {
            T entity = new T();
            Type info = typeof(T);
            var members = info.GetProperties();
            foreach (var mi in members)
            {
                if (mi.Name == "timeStamp")
                    continue;
                var k = "_" + mi.Name;
                if (dr.Table.Columns.Contains(k))
                {
                    if (dr[k].Equals(DBNull.Value))
                    {
                        if (mi.PropertyType == typeof(string) || mi.PropertyType == typeof(char))
                            mi.SetValue(entity, "", null);
                        else if (mi.PropertyType == typeof(long) || mi.PropertyType == typeof(decimal) || mi.PropertyType == typeof(byte)
                            || mi.PropertyType == typeof(sbyte) || mi.PropertyType == typeof(short) || mi.PropertyType == typeof(int)
                            || mi.PropertyType == typeof(ushort) || mi.PropertyType == typeof(uint) || mi.PropertyType == typeof(ulong)
                            || mi.PropertyType == typeof(float) || mi.PropertyType == typeof(double))
                            mi.SetValue(entity, 0, null);
                        else if (mi.PropertyType == typeof(bool))
                            mi.SetValue(entity, false, null);
                        else
                            mi.SetValue(entity, null, null);

                    }
                    else if (dr[k].GetType() == typeof(Guid))
                        mi.SetValue(entity, dr[k].ToString(), null);
                    else if(mi.PropertyType==typeof(Nullable<DateTime>))
                        mi.SetValue(entity, Convert.ChangeType(dr[k], typeof(DateTime)), null);
                    else
                    {
                        try
                        {
                            mi.SetValue(entity, Convert.ChangeType(dr[k], mi.PropertyType), null);
                        }
                        catch (InvalidCastException)
                        {
                            //Console.WriteLine(ex.Message + dr[k].GetType());
                            //如果是枚举要先Parse
                            object obj =Enum.Parse(mi.PropertyType, dr[k].ToString());
                            mi.SetValue(entity, Convert.ChangeType(obj, mi.PropertyType), null);
                        }
                    }
                }

            }
            return entity;
        }

        public static List<T> ToList(DataTable dt)
        {
            List<T> lst = new List<T>(dt.Rows.Count);
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(ToEntity(dr));
            }           
            return lst;
        }

        public static Dictionary<string, object> ToMap(DataRow dr)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (DataColumn dc in dr.Table.Columns)
            {
                var k = dc.ColumnName.Substring(1);
                var type = dc.DataType;
                if (dr[dc.ColumnName].Equals(DBNull.Value))
                {
                    if (type == typeof(string) || type == typeof(char))
                        result.Add(k, "");
                    else if (type == typeof(long) || type == typeof(decimal) || type == typeof(byte)
                        || type == typeof(sbyte) || type == typeof(short) || type == typeof(int)
                        || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong)
                        || type == typeof(float) || type == typeof(double))
                        result.Add(k, 0);
                    else if (type == typeof(bool))
                        result.Add(k, false);
                    else
                        result.Add(k, null);
                }
                else if (dr[dc.ColumnName].GetType() == typeof(Guid))
                    result.Add(k, dr[k].ToString());
                else if (type == typeof(Nullable<DateTime>))
                    result.Add(k,Convert.ChangeType(dr[dc.ColumnName], typeof(DateTime)));
                else
                {
                    result.Add(k, Convert.ChangeType(dr[dc.ColumnName], type));
                }
            }
            return result;
        }

        public static DataTable ToDataTable(List<T> lst)
        {
            if (lst == null)
                return null;
            Type info = typeof(T);
            var members = info.GetProperties();
            DataTable dt = new DataTable();
            foreach (var mi in members)
            {
                var k = "_"+mi.Name;
                if (!dt.Columns.Contains(k))
                {
                    DataColumn dc = null;
                    if (mi.PropertyType==typeof(Nullable<DateTime>))
                        dc = new DataColumn(k, typeof(DateTime));
                    else
                        dc = new DataColumn(k, mi.PropertyType);
                    dt.Columns.Add(dc);
                }
            }

            lst.ForEach(t =>
            {
                var dr = dt.NewRow();
                foreach (var mi in members)
                {
                    var k = "_" + mi.Name;
                    var val= mi.GetValue(t, null);
                    if (val == null)
                        dr[k] = DBNull.Value;
                    else
                        dr[k] = val;
                }
                dt.Rows.Add(dr);
            });
            return dt;
        }

        public static Dictionary<string, string> ToKeyValues(List<T> lst, string key, string value)
        {
            if (lst == null)
                return null;
            Type info = typeof(T);
            var members = info.GetProperties();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string k = "", v = "", str ="";
            lst.ForEach(t =>
            {
                foreach (var mi in members)
                {
                    if (key == mi.Name || value == mi.Name)
                    {                        
                        var val = mi.GetValue(t, null);
                        if (val == null)
                            str = ""; 
                        else
                            str = val.ToString();
                        if (key == mi.Name)
                            k = str;
                        else
                            v = str;
                    }                 
                }
                if (!dict.ContainsKey(k))
                {
                    dict[k] = v;
                }
                
            });
            return dict;
        }

        public static bool HasProperty(string name)
        {
            Type info = typeof(T);
            var rnt=info.GetProperty(name);
            return rnt != null;
        }


        public static void SetProperty(T entity,string name, object value)
        {
            Type info = typeof(T);
            var mi = info.GetProperty(name);
            if (mi == null)
                return;
            try
            {
                mi.SetValue(entity, Convert.ChangeType(value, mi.PropertyType), null);
            }
            catch
            {
                //如果是枚举要先Parse
                object obj = Enum.Parse(mi.PropertyType, value.ToString());
                mi.SetValue(entity, Convert.ChangeType(obj, mi.PropertyType), null);
            }
        }


        public static Dictionary<string,object> EntityToMap(T t)
        {
            var map = new Dictionary<string, object>();
            if (t == null)
                return map;
            Type info = typeof(T);
            var members = info.GetProperties();          
            foreach (var mi in members)
            {
                map.Add(mi.Name, mi.GetValue(t, null));
            }
            return map ;
        }

        public static string PrintString(List<T> lst)
        {
            Type info = typeof(T);
            var members = info.GetProperties();
            var sb = new StringBuilder();
            sb.Append("\r\n");
            foreach (var mi in members)
            {
                sb.Append(mi.Name);
                sb.Append("|\t");
            }
            sb.Append("\r\n");
            foreach (var entity in lst)
            {
                foreach (var mi in members)
                {
                    sb.Append(mi.GetValue(entity).ToString());
                    sb.Append("|\t");
                }
                sb.Append("\r\n");
            }
            
            return sb.ToString();
        }

    }
}
