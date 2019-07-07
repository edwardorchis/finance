using Finance.Account.SDK;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Finance.Account.Service
{
    public class SystemProfileService
    {
        static ILogger logger = Logger.GetLogger(typeof(SystemProfileService));       
        private IDictionary<string, object> mContext;
        public SystemProfileService(IDictionary<string, object> ctx)
        {
            mContext = ctx;
        }

        public static SystemProfileService GetInstance(IDictionary<string, object> ctx)
        {
            return new SystemProfileService(ctx);
        }

        public int GetInt(SystemProfileCategory category, SystemProfileKey key)
        {
            int result = 0;
            if(!int.TryParse(GetString(category,key),out result))
                throw new FinanceException(FinanceResult.IMPERFECT_DATA);
            return result;
        }

        public long GetLong(SystemProfileCategory category, SystemProfileKey key)
        {
            long result = 0;
            if (!long.TryParse(GetString(category, key), out result))
                throw new FinanceException(FinanceResult.IMPERFECT_DATA);
            return result;
        }

        public string GetString(SystemProfileCategory category, SystemProfileKey key)
        {
            return Find(category, key).value;
        }

        public string GetHiddenString(string key)
        {
            var obj = DBHelper.GetInstance(mContext).ExecuteScalar(string.Format("select _value from _systemprofile where _category='{0}' and _key='{1}'", "Hidden", key));
            if (obj == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);

            return obj.ToString();
        }

        public SystemProfile Find(SystemProfileCategory category, SystemProfileKey key)
        {
            var dt = DBHelper.GetInstance(mContext).ExecuteDt(string.Format("select * from _systemprofile where _category='{0}' and _key='{1}'",category,key));
            if (dt == null || dt.Rows.Count == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);

            var rsp = EntityConvertor<SystemProfile>.ToEntity(dt.Rows[0]);           
            return rsp;
        }

        public List<SystemProfile> List()
        { 
            var list = DataManager.GetInstance(mContext).Query<SystemProfile>(null);
            if (list == null || list.Count == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
            return list;
        }

        public void Update(SystemProfileCategory category, SystemProfileKey key,string value)
        {
            var iRnt = 0;            
            iRnt = DBHelper.GetInstance(mContext).ExecuteSql(string.Format("update _systemprofile set _value='{0}' where _category='{1}' and _key='{2}'", value, category, key));            
            if (iRnt == 0)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST);
        }
        public void Update(dynamic tranc, SystemProfileCategory category, SystemProfileKey key, object value)
        {
            var sql = string.Format("update _systemprofile set _value='{0}' where _category='{1}' and _key='{2}'", value, category, key);
            DBHelper.GetInstance(mContext).ExecuteSql(tranc, sql);
        }

        public List<MenuTableMap> AllMenuTables()
        {
            DataTable dt = DBHelper.GetInstance(mContext).ExecuteDt("select * from _MenuTableMap order by _group,_index");
            var lst = EntityConvertor<MenuTableMap>.ToList(dt);
            return lst;
        }

        public List<MenuTableMap> MenuTables()
        {
            var userId = mContext["UserId"].ToString();
            var userName = mContext["UserName"].ToString();

            if (userName == "admin")
                return AllMenuTables();
            else
            {
                var sql = string.Format(@"select menu.* from _MenuTableMap menu right join _AccessRight access on menu._group = access._group and menu._name = access._name
                        where access._id = {0}  and access._mask > 0 order by _group,_index", userId);
                var dt = DBHelper.GetInstance(mContext).ExecuteDt(sql);
                var lst = EntityConvertor<MenuTableMap>.ToList(dt);
                return lst;
            }
            
        }

        public void SaveMenu(MenuTableMap menu)
        {            
            var tran = DBHelper.GetInstance(mContext).BeginTransaction();
            try
            {
                if (DBHelper.GetInstance(mContext).Exist(tran, string.Format("select 1 from _MenuTableMap where _group = '{0}' and _name ='{1}'", menu.group, menu.name)))
                {
                    DBHelper.GetInstance(mContext).ExecuteSql(tran,
                        string.Format("UPDATE [_MenuTableMap] SET [_header] = '{0}',[_financeForm] = '{1}',[_index] = {2} WHERE [_group] ='{3}' and [_name] = '{4}'",
                        menu.header, menu.financeForm,  menu.index, menu.group, menu.name));
                }
                else
                {
                    DBHelper.GetInstance(mContext).ExecuteSql(tran,
                        string.Format("INSERT INTO [_MenuTableMap]([_group],[_name],[_header],[_financeForm],[_index])VALUES('{0}','{1}','{2}','{3}',{4});",
                        menu.group, menu.name, menu.header, menu.financeForm,  menu.index ));
                }
                DBHelper.GetInstance(mContext).CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                DBHelper.GetInstance(mContext).RollbackTransaction(tran);
            }
            
        }

        public DBHelper GetBDBHelper()
        {
            var connectString = GetHiddenString("BConnectString");
            return new DBHelper(connectString);
        }

        public List<AccessRight> GetAccessRightList(long id)
        {
            var access = new AccessRight { id = id };
            var lst = DataManager.GetInstance(mContext).Query(access);
            return lst;
        }

        public void SaveAccessRight(List<AccessRight> lst)
        {
            if (lst.Count == 0)
                return;
            var id = lst.FirstOrDefault().id;
            DataManager.GetInstance(mContext).Delete<AccessRight>(id);
            lst.ForEach(access => { DataManager.GetInstance(mContext).Insert(access); });
        }
    }
}
