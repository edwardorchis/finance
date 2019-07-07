using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public class DataManager
    {
        static IDictionary<long, DataManager> _self = new Dictionary<long, DataManager>();
        private IDictionary<string, object> mContext;
        public DataManager(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static DataManager GetInstance(IDictionary<string, object> ctx)
        {
            long tid = -1;
            if (ctx != null && ctx.ContainsKey("Tid"))
            {
                long.TryParse(ctx["Tid"].ToString(), out tid);
            }
            else
            {
                ctx = new Dictionary<string, object> { { "Tid", -1} };
            }
            if (!_self.ContainsKey(tid))
                _self.Add(tid, new DataManager(ctx));
            return _self[tid];
        }

        public T Find<T>(long id) where T : new()
        {
            T entity = new T();
            Type info = typeof(T);
            var members = info.GetProperties();
            
            foreach (var mi in members)
            {
                if (mi.Name == "id")
                {
                    mi.SetValue(entity, id , null);
                    break;
                }
            }
            var lst = Query<T>(entity);
            if (lst.Count > 0)
            {
                return lst.FirstOrDefault();
            }
            else
            {
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            }
        }

        public void Delete<T>(long id) where T : new()
        {
            bool bhasId = false;
            Type info = typeof(T);
            var members = info.GetProperties();

            foreach (var mi in members)
            {
                if (mi.Name == "id")
                {
                    bhasId = true;
                    break;
                }
            }
            if (!bhasId)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA,string.Format("Class {0} don't has member id, can't delete it by id.",info.Name));
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt(string.Format("delete from _{0} where _id = {1}",info.Name,id));
        }

        public string BuildUpdateSql<T>(T entity) where T : new()
        {
            if (entity == null)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "the object which want to update is null.");
            long id = 0;
            bool bhasId = false;
            object val = null;

            Type info = typeof(T);
            var members = info.GetProperties();

            foreach (var mi in members)
            {
                if (!bhasId && mi.Name == "id")
                {
                    val = mi.GetValue(entity, null);
                    if (!long.TryParse(val.ToString(), out id))
                        throw new FinanceException(FinanceResult.IMPERFECT_DATA, string.Format("id {0} can't parse to long.", val));
                    bhasId = true;
                    break;
                }
            }
            if (!bhasId)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, string.Format("Class {0} don't has member id, can't delete it by id.", info.Name));

            T old = Find<T>(id);

            string udateString = string.Empty;

            foreach (var mi in members)
            {
                if (mi.Name == "timeStamp")
                    continue;
                val = mi.GetValue(entity, null);
                if (val == null)
                    continue;
                object valOld = mi.GetValue(old, null);
                if (!val.Equals(old))
                {
                    if (mi.PropertyType == typeof(long) || mi.PropertyType == typeof(decimal) || mi.PropertyType == typeof(byte)
                    || mi.PropertyType == typeof(sbyte) || mi.PropertyType == typeof(short) || mi.PropertyType == typeof(int)
                    || mi.PropertyType == typeof(ushort) || mi.PropertyType == typeof(uint) || mi.PropertyType == typeof(ulong)
                    || mi.PropertyType == typeof(float) || mi.PropertyType == typeof(double))
                    {

                        udateString = appendupdate(udateString, "_" + mi.Name + "=" + val.ToString());
                    }
                    else if (mi.PropertyType == typeof(bool))
                    {
                        bool newVal = false;
                        if (bool.TryParse(val.ToString(), out newVal))
                        {
                            udateString = appendupdate(udateString, "_" + mi.Name + "=" + (newVal ? 1 : 0));
                        }
                        else
                            throw new FinanceException(FinanceResult.IMPERFECT_DATA, string.Format("object {0} can't parse to bool.", val));
                    }
                    else
                    {
                        udateString = appendupdate(udateString, "_" + mi.Name + "='" + val.ToString() + "'");
                    }
                }
            }

            if (!string.IsNullOrEmpty(udateString))
            {
                return string.Format("update _{0} set {1} where _id = {2}", info.Name, udateString, id);
            }
            throw new FinanceException(FinanceResult.SYSTEM_ERROR);
        }

        public void Update<T>(T entity) where T : new()
        {           
            DBHelper.GetInstance(mContext).ExecuteSql(BuildUpdateSql(entity));
        }

        public void Insert<T>(T entity) where T : new()
        {
            if (entity == null)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "the object which want to update is null.");

            Type info = typeof(T);

            DataTable dt = EntityConvertor<T>.ToDataTable(new List<T>() { entity });
            DBHelper.GetInstance(mContext).InsertTable(dt,"_" + info.Name);
        }

        public List<T> Query<T>(T entity ) where T : new()
        {
            Type info = typeof(T);
            var members = info.GetProperties();
            string filter = string.Empty;
            if (entity != null)
            {
                object val = null;
                foreach (var mi in members)
                {
                    if (mi.PropertyType == typeof(string) || mi.PropertyType == typeof(char))
                    {
                        val = mi.GetValue(entity, null);
                        if (val != null && !val.Equals(string.Empty))
                        {
                            filter = appendfilter(filter, "_" + mi.Name + " = '" + val + "'");
                        }
                    }
                    else if (mi.PropertyType == typeof(long) || mi.PropertyType == typeof(decimal) || mi.PropertyType == typeof(byte)
                        || mi.PropertyType == typeof(sbyte) || mi.PropertyType == typeof(short) || mi.PropertyType == typeof(int)
                        || mi.PropertyType == typeof(ushort) || mi.PropertyType == typeof(uint) || mi.PropertyType == typeof(ulong)
                        || mi.PropertyType == typeof(float) || mi.PropertyType == typeof(double))
                    {
                        val = mi.GetValue(entity, null);
                        if (!(decimal.Parse(val.ToString())).Equals(decimal.Zero))
                        {
                            filter = appendfilter(filter, "_" + mi.Name + " = " + val + "");
                        }

                    }
                    else
                    {

                    }
                }
            }
            string tableName = "_" + info.Name;
            string sql = string.Format("select * from {0} ",tableName);
            if (!string.IsNullOrEmpty(filter))
                sql += " where " + filter;

            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt(sql);
            return EntityConvertor<T>.ToList(dt);
        }

        static string appendfilter(string orialfilter,string appendfilter)
        {
            if (string.IsNullOrEmpty(orialfilter))
                return appendfilter;
            else
                return orialfilter + " AND " + appendfilter;
        }

        static string appendupdate(string orial, string src)
        {
            if (string.IsNullOrEmpty(orial))
                return src;
            else
                return orial + " , " + src;
        }




        public List<T> Query<T>(dynamic tran,T entity) where T : new()
        {
            Type info = typeof(T);
            var members = info.GetProperties();
            string filter = string.Empty;
            if (entity != null)
            {
                object val = null;
                foreach (var mi in members)
                {
                    if (mi.PropertyType == typeof(string) || mi.PropertyType == typeof(char))
                    {
                        val = mi.GetValue(entity, null);
                        if (val != null && !val.Equals(string.Empty))
                        {
                            filter = appendfilter(filter, "_" + mi.Name + " = '" + val + "'");
                        }
                    }
                    else if (mi.PropertyType == typeof(long) || mi.PropertyType == typeof(decimal) || mi.PropertyType == typeof(byte)
                        || mi.PropertyType == typeof(sbyte) || mi.PropertyType == typeof(short) || mi.PropertyType == typeof(int)
                        || mi.PropertyType == typeof(ushort) || mi.PropertyType == typeof(uint) || mi.PropertyType == typeof(ulong)
                        || mi.PropertyType == typeof(float) || mi.PropertyType == typeof(double))
                    {
                        val = mi.GetValue(entity, null);
                        if (!(decimal.Parse(val.ToString())).Equals(decimal.Zero))
                        {
                            filter = appendfilter(filter, "_" + mi.Name + " = " + val + "");
                        }

                    }
                    else
                    {

                    }
                }
            }
            string tableName = "_" + info.Name;
            string sql = string.Format("select * from {0} ", tableName);
            if (!string.IsNullOrEmpty(filter))
                sql += " where " + filter;

            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt(tran,sql);
            return EntityConvertor<T>.ToList(dt);
        }

        public void Update<T>(dynamic tran, T entity) where T : new()
        {
            DBHelper.GetInstance(mContext).ExecuteSql(tran,BuildUpdateSql(entity));
        }

        public void Insert<T>(dynamic tran, T entity) where T : new()
        {
            if (entity == null)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "the object which want to update is null.");

            Type info = typeof(T);

            DataTable dt = EntityConvertor<T>.ToDataTable(new List<T>() { entity });
            DBHelper.GetInstance(mContext).InsertTable(tran, dt, "_" + info.Name);
        }

        public void InsertList<T>(dynamic tran, List<T> list) where T : new()
        {
            if (list == null || list.Count == 0)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA, "the object which want to update is null or empty.");

            Type info = typeof(T);

            DataTable dt = EntityConvertor<T>.ToDataTable(list);
            DBHelper.GetInstance(mContext).InsertTable(tran, dt, "_" + info.Name);
        }





    }
}
