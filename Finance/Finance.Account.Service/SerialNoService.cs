using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public class SerialNoService
    {
        private IDictionary<string, object> mContext;
        public SerialNoService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }
        dynamic m_tran = null;
        long MID = 0;

        public SerialNoService(IDictionary<string, object> ctx, dynamic tran)
        {
            mContext = ctx;
            m_tran = tran;
        }

        public static SerialNoService GetInstance(IDictionary<string, object> ctx)
        {
            return new SerialNoService(ctx);
        }


        public long Get(SerialNoKey key, string ex = "")
        {          
            var obj = DBHelper.GetInstance(mContext).ExecuteScalar(string.Format("select _number from _SerialNo where _key ={0} and _ex='{1}' ", (int)key, ex));
            if (obj == null)
                return 1;
            else
                return (long)obj;
        }

        public void Update(SerialNoKey key, string ex = "")
        {
            DBHelper.GetInstance(mContext).ExecuteSql(string.Format("update _SerialNo set _number=_number + 1 where _key={0} and _ex='{1}'", (int)key, ex));
        }

        public long GetIncrease(long count, SerialNoKey key, string ex = "")
        {
            long id = 0;
            var db = DBHelper.GetInstance(mContext);
            var tran = db.BeginTransaction();
            try
            {
                DataTable dt = db.ExecuteDt(tran, string.Format("select _number from _SerialNo where _key ={0} and _ex='{1}' ", (int)key, ex));
                if (dt.Rows.Count == 0)
                {
                    id = 1;
                    db.ExecuteSql(tran, string.Format("insert into _SerialNo(_key,_ex ,_number) values ({0},'{1}',{2})", (int)key, ex, id + count));
                }
                else
                {
                    id = (long)dt.Rows[0][0];
                    db.ExecuteSql(tran, string.Format("update _SerialNo set _number = {2} where _key = {0} and _ex='{1}'", (int)key, ex, id + count));
                }
                db.CommitTransaction(tran);
            }
            catch (Exception e)
            {
                db.RollbackTransaction(tran);
                throw e;
            }
            return id;
        }

        public SerialNoKey SerialKey { set; get; }
        public string Ex { set; get; } = string.Empty;

        /// <summary>
        /// 取连续id使用该方法，必须和UpdateWithTrans配套
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public long First()
        {
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt(m_tran, string.Format("select _number from _SerialNo where _key ={0} and _ex='{1}' ", (int)SerialKey, Ex));
            if (dt.Rows.Count == 0)
            {
                MID = 1;
                DBHelper.GetInstance(mContext).ExecuteSql(m_tran, string.Format("insert into _SerialNo(_key,_ex,_number) values ({0},'{1}',{2})", (int)SerialKey, Ex, MID));
            }
            else
            {
                MID = (long)dt.Rows[0][0];
            }
            return MID;
        }

        public long Increase()
        {
            return ++MID;
        }

        public void Update()
        {
            DBHelper.GetInstance(mContext).ExecuteSql(m_tran, string.Format("update _SerialNo set _number = {2} where _key = {0} and _ex = '{1}'", (int)SerialKey, Ex, MID + 1));
        }
        public long GetIncrease()
        {
            long id = 0;
           
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt(m_tran, string.Format("select _number from _SerialNo where _key ={0} and _ex='{1}' ", (int)SerialKey, Ex));
            if (dt.Rows.Count == 0)
            {
                id = 1;
                DBHelper.GetInstance(mContext).ExecuteSql(m_tran, string.Format("insert into _SerialNo(_key,_ex ,_number) values ({0},'{1}',{2})", (int)SerialKey, Ex, id + 1));
            }
            else
            {
                id = (long)dt.Rows[0][0];
                DBHelper.GetInstance(mContext).ExecuteSql(m_tran, string.Format("update _SerialNo set _number = {2} where _key = {0} and _ex='{1}'", (int)SerialKey, Ex, id + 1));
            } 
            return id;
        }

        public static string GetUUID()
        {
            return System.Guid.NewGuid().ToString("N");
        }

    }
}
