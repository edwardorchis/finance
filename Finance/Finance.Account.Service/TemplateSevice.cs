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
    public class TemplateSevice
    {
        static ILogger logger = Logger.GetLogger(typeof(TemplateSevice));
        private IDictionary<string, object> mContext;
        private DBHelper mDBHelper = null;
        public TemplateSevice(IDictionary<string, object> ctx)
        {
            mContext = ctx;
            mDBHelper = DBHelper.GetInstance(mContext);
        }

        public static TemplateSevice GetInstance(IDictionary<string, object> ctx)
        {
            return new TemplateSevice(ctx);
        }


        public List<ExcelTemplateItem> FindTemplate(string name)
        {
            DataTable dt = mDBHelper.ExecuteDt(string.Format("select * from _ExcelTemplate where _name ='{0}'", name));
            var lst = EntityConvertor<ExcelTemplateItem>.ToList(dt);
            return lst;
        }

        public List<UdefTemplateItem> FindUdefTemplate(string name)
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(name))
                dt = mDBHelper.ExecuteDt(string.Format("select * from _UdefTemplate where _tableName ='{0}'", name));
            else 
                dt = mDBHelper.ExecuteDt(string.Format("select * from _UdefTemplate order by _tableName ,_reserved,_tabIndex"));
            var lst = EntityConvertor<UdefTemplateItem>.ToList(dt);

            lst.ForEach(item=> {
                var val = item.defaultVal;
                var str = val.ToString();
                if (str.StartsWith("$") && str.IndexOf("(") != -1 && str.LastIndexOf(")") > 0)
                {
                    str = str.Substring(str.IndexOf("(") + 1, str.LastIndexOf(")") - str.IndexOf("(") - 1);
                    switch (str)
                    {
                        case "currentYear":
                            item.defaultVal = SystemProfileService.GetInstance(mContext).GetString(SystemProfileCategory.Account ,SystemProfileKey.CurrentYear);
                            break;
                        case "currentPeriod":
                            item.defaultVal = SystemProfileService.GetInstance(mContext).GetString(SystemProfileCategory.Account, SystemProfileKey.CurrentPeriod);
                            break;
                    }
                }
            });

            return lst;
        }

        public void SaveUdeftemplate(UdefTemplateItem udefTemplate)
        {
            var tran = mDBHelper.BeginTransaction();
            try
            {
                if (mDBHelper.Exist(tran, string.Format("select 1 from _UdefTemplate where _tableName = '{0}' and _name ='{1}'", udefTemplate.tableName, udefTemplate.name)))
                {
                    mDBHelper.ExecuteSql(tran,
                        string.Format("UPDATE [_UdefTemplate] SET [_label] = '{0}',[_dataType] = '{1}',[_tabIndex] = '{2}',[_defaultVal] = '{3}',[_reserved] = '{4}',[_tagLabel] = '{5}',[_width] = {6} WHERE [_tableName] = '{7}'and [_name] = '{8}'",
                        udefTemplate.label, udefTemplate.dataType, udefTemplate.tabIndex, udefTemplate.defaultVal, udefTemplate.reserved, udefTemplate.tagLabel, udefTemplate.width, udefTemplate.tableName, udefTemplate.name));
                }
                else
                {
                    mDBHelper.ExecuteSql(tran,
                        string.Format("INSERT INTO [_UdefTemplate]([_tableName],[_name],[_label],[_dataType],[_tabIndex],[_defaultVal],[_reserved],[_tagLabel],[_width]) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8})",
                        udefTemplate.tableName, udefTemplate.name, udefTemplate.label, udefTemplate.dataType, udefTemplate.tabIndex, udefTemplate.defaultVal, udefTemplate.reserved, udefTemplate.tagLabel, udefTemplate.width));
                }
                mDBHelper.CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                mDBHelper.RollbackTransaction(tran);
                throw ex;
            }


        }

        public void DeleteUdeftemplate(UdefTemplateItem udefTemplate)
        {
            mDBHelper.ExecuteSql(string.Format("delete from _UdefTemplate where _tableName = '{0}' and _name = '{1}'", udefTemplate.tableName, udefTemplate.name));
        }


        public List<CarriedForwardTemplate> ListCarriedForwardTemplate(long id)
        {
            DataTable dt = mDBHelper.ExecuteDt(string.Format("select * from _CarriedForwardTemplate where _id ={0}", id));
            var lst = EntityConvertor<CarriedForwardTemplate>.ToList(dt);
            return lst;
        }

        public void SaveCarriedForwardTemplate(List<CarriedForwardTemplate> list)
        {
            var ids = list.Select(c => c.id).Distinct();
            if (ids.Count() > 1)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA);

            var id = ids.First();
            var tran = mDBHelper.BeginTransaction();
            try
            {
                mDBHelper.ExecuteSql(tran, string.Format("delete from _CarriedForwardTemplate where _id ={0}", id));
                DataManager.GetInstance(mContext).InsertList(tran, list);
                mDBHelper.CommitTransaction(tran);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                mDBHelper.RollbackTransaction(tran);
                throw ex;
            }
        }
    }
}
