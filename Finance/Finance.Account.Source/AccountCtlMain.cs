
using Finance.Account.Source.Struct;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Source
{
    public class AccountCtlMain
    {
        static Utils.ILogger logger()
        {
            return Logger.GetLogger(typeof(AccountCtlMain));
        }

        public static int Test()
        {
            try
            {
                if (DBHelper.DefaultInstance.Exist("select 1 from sysobjects where id = object_id('_AccountCtl') and OBJECTPROPERTY(id, N'IsUserTable') = 1"))
                {
                    return 0;
                }                
            }
            catch
            {
            }
            return 255;
        }

        public static void Init()
        {
            try
            {
                var sql = Generator.GenerateSql(typeof(AccountCtlData));
                DBHelper.DefaultInstance.ExecuteSql(sql);
                logger().Info("Init finance db done ...");
                logger().Info("Init demo ...");
                InitData();
                logger().Info("Init demo done ...");
                logger().Info("Init AccountCtl data success.");
            }
            catch (FinanceException e)
            {
                logger().Error(e.Message);
            }
        }
  
        public static void InitData()
        {    
            CreateDB("finance_demo");

            var defaultConnectString = ConfigHelper.Instance.XmlReadConnectionString("default");

            var demoConnectString = BuildConnectString("finance_demo");

            DataManager.GetInstance(null).Insert(new AccountCtl { connstr= demoConnectString, id = 0, no = "finance_demo", name = "演示账套", createTime = DateTime.Now});
            DataManager.GetInstance(null).Insert(new AccountUser { id = 0, no = "admin", name = "管理员", pwd = "E10ADC3949BA59ABBE56E057F20F883E", lastLoginTime = DateTime.Now });

            SourceMain.Init(0);
        }

        public static string BuildConnectString(string dbname)
        {
            var defaultConnectString = ConfigHelper.Instance.XmlReadConnectionString("default");
            var tmp = defaultConnectString.Split(';');
            int i = 0;
            for (; i < tmp.Length; i++)
            {
                if (tmp[i].StartsWith("Initial Catalog"))
                    break;
            }
            tmp[i] = "Initial Catalog=" + dbname;
            return string.Join(";", tmp);
        }

        public static void CreateDB(string name)
        {
            var sql = @"if not exists(select 1 from master.sys.databases t where t.name='{0}')
begin
    create database {0} 
    on  primary 
    (
        name='{0}_data',  
        filename='{1}\DBFiles\{0}_data.mdf',
        size=5mb, 
        maxsize=100mb,
        filegrowth=15%
    )
    log on
    (
        name='{0}_log',
        filename='{1}\DBFiles\{0}_log.ldf',
        size=2mb,
        filegrowth=1mb
    )
end";
            sql = string.Format(sql, name, Generator.getSourcePath()) ;
            DBHelper.DefaultInstance.ExecuteSqlNoTran(sql);
        }


        public static void CreateAccount(string no, string name)
        {
            if (DBHelper.DefaultInstance.Exist(string.Format("select 1 from master.sys.databases t where t.name='{0}'", no)))
                throw new FinanceException(FinanceResult.RECORD_EXIST, string.Format("账套[{0}]已存在",no));
            CreateDB(no);
            if (DBHelper.DefaultInstance.Exist(string.Format("select 1 from _AccountCtl where _no='{0}'", no)))
                DBHelper.DefaultInstance.ExecuteSql(string.Format("delete from _AccountCtl where _no='{0}'", no));
            long id = 1;
            var maxId = DBHelper.DefaultInstance.ExecuteScalar("select max(_id) from _AccountCtl");
            if (maxId != null)
            {
                if (long.TryParse(maxId.ToString(), out id))
                    id++;
            }               

            DataManager.GetInstance(null).Insert(new AccountCtl { connstr = BuildConnectString(no), id = id, no = no, name = name, createTime = DateTime.Now });
            SourceMain.Init(id);
        }

        public static void LoadAccount(string no, string name)
        {
            if (!DBHelper.DefaultInstance.Exist(string.Format("select 1 from master.sys.databases t where t.name='{0}'", no)))
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, string.Format("账套[{0}]不存在", no));
            if (DBHelper.DefaultInstance.Exist(string.Format("select 1 from _AccountCtl where _no='{0}'", no)))
                return;
            long id = 1;
            var maxId = DBHelper.DefaultInstance.ExecuteScalar("select max(_id) from _AccountCtl");
            if (maxId != null)
            {
                if (long.TryParse(maxId.ToString(), out id))
                    id++;
            }

            DataManager.GetInstance(null).Insert(new AccountCtl { connstr = BuildConnectString(no), id = id, no = no, name = name, createTime = DateTime.Now });
        }

        public static void UnloadAccount(string no)
        {
            if (!DBHelper.DefaultInstance.Exist(string.Format("select 1 from _AccountCtl where _no='{0}'", no)))
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, string.Format("账套[{0}]不存在", no));
            DBHelper.DefaultInstance.ExecuteSql(string.Format("delete from _AccountCtl where _no='{0}'", no));
        }

        public static void InitAccount(string no, string param = "")
        {
            if (!DBHelper.DefaultInstance.Exist(string.Format("select 1 from master.sys.databases t where t.name='{0}'", no)))
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, string.Format("账套[{0}]不存在", no));

            var idObj = DBHelper.DefaultInstance.ExecuteScalar(string.Format("select _id from _AccountCtl where _no='{0}'", no));
            if (idObj == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, string.Format("账套[{0}]未加载", no));

            if (param.Trim() == "-k")
            {
                var db = DBHelper.GetInstance(new Dictionary<string, object> { { "Tid", (long)idObj } });
                db.ExecuteSql(@"delete from _VoucherEntryUdef; delete from _VoucherEntry; delete from _VoucherHeader;
delete from _TaskResult;delete from _OperationLog;delete from _SerialNo where _key > 0;
update _SystemProfile set _value = 0 where _category= 'Account' and _key = 'IsInited';");
            }
            else
            {
                SourceMain.Init((long)idObj);
            }
        }

        public static void AccoutPrint()
        {
            var lst = DataManager.GetInstance(null).Query<AccountCtl>(null);
            var str = EntityConvertor<AccountCtl>.PrintString(lst);
            logger().Info(str);
        }

        public static bool Verification(string no, string pwd)
        {
            pwd = CryptInfoHelper.MD5Encode(pwd);
            if (DBHelper.DefaultInstance.Exist(string.Format("select 1 from _AccountUser where _no = '{0}' and _pwd = '{1}'", no, pwd)))
            {
                return true;
            }
            return false;
        }

        public static void ChagePwd(string no, string pwd)
        {
            pwd = CryptInfoHelper.MD5Encode(pwd);
            DBHelper.DefaultInstance.ExecuteSql(string.Format("update _AccountUser set _pwd = '{1}' where _no = '{0}'", no, pwd));
        }

        public static void AddUser(string no,string name, string pwd)
        {
            if (DBHelper.DefaultInstance.Exist(string.Format("select 1 from _AccountUser where _no = '{0}'", no)))
            {
                throw new FinanceException(FinanceResult.RECORD_EXIST);
            }
            pwd = CryptInfoHelper.MD5Encode(pwd);
            long id = 1;
            var maxId = DBHelper.DefaultInstance.ExecuteScalar("select max(_id) from _AccountUser");
            if (maxId != null)
            {
                if (long.TryParse(maxId.ToString(), out id))
                    id++;
            }
            DBHelper.DefaultInstance.ExecuteSql(string.Format(@"INSERT INTO [_AccountUser]([_id],[_no],[_name],[_pwd],[_lastLoginTime])  
VALUES({0},'{1}','{2}','{3}', GETDATE())", id, no, name, pwd));
        }

        public static void DeleteUser(string no)
        {
            if (no == "admin")
                throw new Exception("Can't delete admin.");
            var idObj = DBHelper.DefaultInstance.ExecuteScalar(string.Format("select _id from _AccountUser where _no='{0}'", no));
            if (idObj == null)
                throw new FinanceException(FinanceResult.RECORD_NOT_EXIST, string.Format("用户[{0}]不存在", no));
            DBHelper.DefaultInstance.ExecuteSql("delete from _AccountUser where _id = " + idObj.ToString());
        }

        public static void UserPrint()
        {
            var dt = DBHelper.DefaultInstance.ExecuteDt("SELECT [_id] as id,[_no] as no,[_name] as name,[_lastLoginTime] as lastLoginTime from [_AccountUser]");
            var sb = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                sb.Append("\r\n");
                foreach (DataColumn dc in dt.Columns)
                {
                    sb.Append(dc.ColumnName);
                    sb.Append("|\t");
                }
                sb.Append("\r\n");

                foreach(DataRow dr in dt.Rows)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        sb.Append(dr[dc.ColumnName]);
                        sb.Append("|\t");
                    }
                    sb.Append("\r\n");
                }
            }

            logger().Info(sb.ToString());
        }
    }
}
