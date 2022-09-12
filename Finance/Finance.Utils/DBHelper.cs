using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public class DBHelper : IDisposable
    {
        static Dictionary<long, DBHelper> _instanceList = new Dictionary<long, DBHelper>();

        public static DBHelper DefaultInstance
        {
            get
            {
                if (!_instanceList.ContainsKey(-1))
                    _instanceList.Add(-1, new DBHelper(ConfigHelper.Instance.XmlReadConnectionString("default")));
                return _instanceList[-1];
            }
            set
            {
                if (!_instanceList.ContainsKey(-1))
                    _instanceList.Add(-1, value);
                else
                    _instanceList[-1] = value;
            }
        } 

        public static DBHelper GetInstance(IDictionary<string,object> ctx)
        {
            long tid = 0;
            long.TryParse(ctx["Tid"].ToString(), out tid);
            if (!_instanceList.ContainsKey(tid))
            {
                if (tid == -1)
                    return DefaultInstance;

                var conStr =  DefaultInstance.ExecuteScalar("select _connstr from _AccountCtl where _id = " + tid);
                _instanceList.Add(tid, new DBHelper(conStr.ToString()));
            }
                
            return _instanceList[tid];
        }

        #region 定义
        private object _lock = null;
        private SqlConnection con;
        private string strConnection;

        public DBHelper(string connectString)
        {
            _lock = new object();
            strConnection = connectString;
        }


       
        #endregion 定义

        #region 通用方法

        #region ---------------返回数据库连接字符串--------------

        /// <summary>
        /// 返回数据库连接字符串
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public String GetSqlConnection()
        {
            String conn = strConnection;
            return conn;
        }

        #endregion ---------------返回数据库连接字符串--------------

        #region ---------------执行不带参数的SQL语句--------------

        /// <summary>
        /// 执行不带参数的SQL语句
        /// </summary>
        /// <param name="Sqlstr">Sql语句</param>
        /// <returns></returns>
        public int ExecuteSql(String Sqlstr)
        {

            String ConnStr = GetSqlConnection();
            int i;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandTimeout = 120;
                        cmd.Transaction = trans;
                        cmd.Connection = conn;
                        cmd.CommandText = Sqlstr;
                        i = cmd.ExecuteNonQuery();
                        trans.Commit();
                        return i;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }

        }

        public int ExecuteSqlByZero(String Sqlstr)
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                int i;
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandTimeout = 3600;
                            cmd.Transaction = trans;
                            cmd.Connection = conn;
                            cmd.CommandText = Sqlstr;
                            i = cmd.ExecuteNonQuery();
                            trans.Commit();
                            return i;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }
        #endregion ---------------执行不带参数的SQL语句--------------

        #region ---------------执行带参数的SQL语句--------------

        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="Sqlstr">SQL语句</param>
        /// <param name="param">参数对象数组</param>
        /// <returns></returns>
        public int ExecuteSql(String Sqlstr, SqlParameter[] param)
        {
            lock (_lock)
            {
                int iResult = 0;
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Transaction = trans;
                            cmd.Connection = conn;
                            cmd.CommandText = Sqlstr;
                            cmd.Parameters.AddRange(param);
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                            cmd.Dispose();
                            iResult = 1;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
                return iResult;
            }
        }

        #endregion ---------------执行带参数的SQL语句--------------

        #region ---------------执行多条SQL语句--------------
        public object ExecuteScalar(string sqltxt)
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sqltxt;
                    return cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句
        /// </summary>
        /// <param name="SQLStringList">Sql集合</param>
        /// <returns></returns>
        public int ExecuteSql(Dictionary<string, object> SQLStringList)
        {
            lock (_lock)
            {
                int iResult = 0;
                //StringBuilder sb = new StringBuilder();
                String ConnStr = GetSqlConnection();

                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int result = 0;
                            SqlCommand cmd = new SqlCommand();
                            cmd.Transaction = trans;
                            cmd.Connection = conn;
                            //循环执行sql语句
                            foreach (KeyValuePair<string, object> entry in SQLStringList)
                            {
                                cmd.CommandText = entry.Key.ToString().Substring(0, entry.Key.ToString().IndexOf("|*|"));
                                cmd.Parameters.AddRange((SqlParameter[])entry.Value);
                                //string strSqlValue = String.Empty;
                                //foreach (SqlParameter sp in cmd.Parameters)
                                //{

                                //    strSqlValue += sp.ParameterName + ": " + sp.SqlValue + "|";

                                //}
                                if (conn.State == ConnectionState.Closed) conn.Open();
                                //LogWirter lw = new LogWirter();
                                //lw.EventLogType = System.Diagnostics.EventLogEntryType.Information;
                                //lw.EventSourceName = "SQL";
                                //lw.LogEvent(strSqlValue);
                                result = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                result = 1;
                            }
                            if (result == 1)
                            {
                                trans.Commit();
                            }
                            iResult = 1;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();

                            throw ex;
                        }
                    }

                }

                return iResult;
            }
        }

        #endregion ---------------执行多条SQL语句--------------

        #region ---------------带事务执行sql语句--------------

        /// <summary>
        /// 带事务执行sql语句
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="cmd">执行命令</param>
        /// <param name="conn">数据连接</param>
        /// <param name="strSql">执行Sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public int ExecuteSql(SqlTransaction trans, SqlCommand cmd, SqlConnection conn, string strSql, SqlParameter[] param)
        {
            lock (_lock)
            {
                int iResult = 0;
                try
                {
                    cmd.Transaction = trans;
                    cmd.Connection = conn;
                    cmd.CommandText = strSql;
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                    iResult = 1;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                return iResult;
            }
        }

        #endregion ---------------带事务执行sql语句--------------

        #region ---------------返回DataReader--------------

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <param name="Sqlstr">执行Sql</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(String Sqlstr)
        {
            lock (_lock)
            {

                String ConnStr = GetSqlConnection();
                SqlConnection conn = new SqlConnection(ConnStr);//返回DataReader时,是不可以用using()的
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = Sqlstr;
                    conn.Open();
                    return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);//关闭关联的Connection
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion ---------------返回DataReader--------------

        #region ---------------执行SQL语句并返回DATATABLE--------------

        /// <summary>
        /// 执行SQL语句并返回DATATABLE
        /// </summary>
        /// <param name="Sqlstr">SQL语句</param>
        /// <returns></returns>
        public DataTable ExecuteDt(String Sqlstr)
        {

            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
                da.SelectCommand.CommandTimeout = 300;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }

        }

        #endregion ---------------执行SQL语句并返回DATATABLE--------------

        #region ---------------执行SQL语句并返回DataSet--------------

        /// <summary>
        /// 执行SQL语句并返回DataSet
        /// </summary>
        /// <param name="Sqlstr">SQL语句</param>
        /// <returns></returns>
        public DataSet ExecuteDs(String Sqlstr)
        {

            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }

        }

        #endregion ---------------执行SQL语句并返回DataSet--------------



        public void InsertTable(DataTable dt, string tableName)
        {
            lock (_lock)
            {
                SqlConnection conn = new SqlConnection(GetSqlConnection());
                conn.Open();
                using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                {
                    sqlBC.BatchSize = 1000;
                    sqlBC.BulkCopyTimeout = 60;
                    //sqlBC.NotifyAfter = 10000;
                    sqlBC.DestinationTableName = tableName;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        sqlBC.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                    }
                    sqlBC.WriteToServer(dt);
                }
                conn.Close();
            }
        }





        #endregion 通用方法

        #region 存储过程

        /// <summary>
        /// 运行存储过程(已重载)
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <returns>存储过程的返回值</returns>
        public object RunProc(string procName)
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    SqlCommand cmd = CreateCommand(procName, null);
                    cmd.ExecuteNonQuery();
                    return cmd.Parameters["ReturnValue"].Value;
                }
            }
        }

        /// <summary>
        /// 运行存储过程(已重载)
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <param name="prams">存储过程的输入参数列表</param>
        /// <returns>存储过程的返回值</returns>
        public object RunProc(string procName, SqlParameter[] prams)
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    SqlCommand cmd = CreateCommand(procName, prams);
                    cmd.ExecuteNonQuery();
                    return cmd.Parameters[1].Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="prams"></param>
        /// <returns></returns>
        public DataSet RunDataSetProc(string procName, SqlParameter[] prams = null)
        {
            lock (_lock)
            {
                SqlCommand cmd = CreateCommand(procName, prams);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);              
                return ds;
            }
        }

        public object RunProcScalar(string procName, SqlParameter[] prams = null)
        {
            var dt = RunTableProc(procName, prams);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0][0];
        }

        /// <summary>
        /// 运行存储过程(已重载)
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <param name="prams">存储过程的输入参数列表</param>
        /// <param name="dataReader">结果集</param>
        /// <returns></returns>
        public DataTable RunTableProc(string procName, SqlParameter[] prams)
        {
            lock (_lock)
            {

                SqlCommand cmd = CreateCommand(procName, prams);
                DataTable dt = new DataTable();
                using (SqlDataReader dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    for (int i = 0; i < dataReader.FieldCount; ++i)
                    {
                        dt.Columns.Add(dataReader.GetName(i), dataReader.GetFieldType(i));
                    }
                    object[] temp = new object[dataReader.FieldCount];
                    while (dataReader.Read())
                    {
                        dataReader.GetValues(temp);
                        dt.LoadDataRow(temp, true);
                    }
                    dataReader.Close();
                }

                return dt;
            }
        }

        /// <summary>
        /// 运行存储过程(已重载)
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <param name="dataReader">结果集</param>
        /// <returns></returns>
        public DataTable RunTableProc(string procName)
        {
            lock (_lock)
            {
                SqlCommand cmd = CreateCommand(procName, null);
                DataTable dt = new DataTable();
                using (SqlDataReader dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    for (int i = 0; i < dataReader.FieldCount; ++i)
                    {
                        dt.Columns.Add(dataReader.GetName(i), dataReader.GetFieldType(i));
                    }
                    object[] temp = new object[dataReader.FieldCount];
                    while (dataReader.Read())
                    {
                        dataReader.GetValues(temp);
                        dt.LoadDataRow(temp, true);
                    }
                }

                return dt;
            }
        }

        /// <summary>
        /// 运行存储过程(已重载)
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <param name="prams">存储过程的输入参数列表</param>
        /// <param name="dataReader">结果集</param>
        /// <returns></returns>
        public void RunProc(string procName, SqlParameter[] prams, out SqlDataReader dataReader)
        {
            lock (_lock)
            {

                SqlCommand cmd = CreateCommand(procName, prams);
                dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
        }

        /// <summary>
        /// 创建Command对象用于访问存储过程
        /// </summary>
        /// <param name="procName">存储过程的名字</param>
        /// <param name="prams">存储过程的输入参数列表</param>
        /// <returns>Command对象</returns>
        private SqlCommand CreateCommand(string procName, SqlParameter[] prams)
        {
            lock (_lock)
            {
                // 确定连接是打开的
                Open();
                String ConnStr = GetSqlConnection();
                SqlCommand cmd = new SqlCommand(procName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                // 添加存储过程的输入参数列表
                if (prams != null)
                {
                    foreach (SqlParameter parameter in prams)
                        cmd.Parameters.Add(parameter);
                }
                // 返回Command对象
                return cmd;
            }
        }

        /// <summary>
        /// 创建输入参数
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Value">参数值</param>
        /// <returns>新参数对象</returns>
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// 创建输出参数
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新参数对象</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// 创建存储过程参数
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Direction">参数的方向(输入/输出)</param>
        /// <param name="Value">参数值</param>
        /// <returns>新参数对象</returns>
        public SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            lock (_lock)
            {
                SqlParameter param;
                if (Size > 0)
                {
                    param = new SqlParameter(ParamName, DbType, Size);
                }
                else
                {
                    param = new SqlParameter(ParamName, DbType);
                }
                param.Direction = Direction;
                if (!(Direction == ParameterDirection.Output && Value == null))
                {
                    param.Value = Value;
                }
                return param;
            }
        }

        public int ExecuteSqlNoTran(String Sqlstr)
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                int i;
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandTimeout = 3600;
                    cmd.Connection = conn;
                    cmd.CommandText = Sqlstr;
                    i = cmd.ExecuteNonQuery();
                    return i;
                }
            }
        }
        #endregion 存储过程

        #region 数据库连接和关闭

        /// <summary>
        /// 测试打开数据库
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public bool IsOpen()
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    return true;
                }
            }
        }

        /// <summary>
        /// 打开连接池
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public void Open()
        {
            lock (_lock)
            {
                // 打开连接池
                if (con == null)
                {
                    //这里不仅需要using System.Configuration;还要在引用目录里添加
                    con = new SqlConnection(GetSqlConnection());
                    con.Open();
                }
                else if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
            }
        }

        /// <summary>
        /// 关闭连接池
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public void Close()
        {
            lock (_lock)
            {
                if (con != null)
                    con.Close();
            }
        }

        /// <summary>
        /// 释放连接池
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public void Dispose()
        {
            lock (_lock)
            {
                // 确定连接已关闭
                if (con != null)
                {
                    con.Dispose();
                    con = null;
                }
            }
        }

        #endregion 数据库连接和关闭

        public bool Exist(string sqlstr)
        {
            var dt = ExecuteDt(sqlstr);
            return dt != null && dt.Rows.Count > 0;
        }
        #region 事务
        /// <summary>
        /// 活动一个连接事务对象，包含conn和trans
        /// </summary>
        /// <returns></returns>
        public dynamic BeginTransaction()
        {
            dynamic tran = new System.Dynamic.ExpandoObject();
            con = new SqlConnection(GetSqlConnection());
            con.Open();
            tran.conn = con;
            tran.trans = tran.conn.BeginTransaction();
            return tran;
        }
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="tran">BeginTransaction返回的对象</param>
        /// <param name="Sqlstr"></param>
        /// <param name="param"></param>
        public int ExecuteSql(dynamic tran, string Sqlstr, SqlParameter[] param = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = tran.trans;
            cmd.Connection = tran.conn;
            cmd.CommandText = Sqlstr;
            if (param != null)
                cmd.Parameters.AddRange(param);
            if (tran.conn.State == ConnectionState.Closed)
                tran.conn.Open();
            return cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="tran">BeginTransaction返回的对象</param>
        /// <param name="Sqlstr"></param>
        /// <returns></returns>
        public DataTable ExecuteDt(dynamic tran, string Sqlstr)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            cmd.Transaction = tran.trans;
            cmd.Connection = tran.conn;
            cmd.CommandText = Sqlstr;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void InsertTable(dynamic tran, DataTable dt, string tableName)
        {
            var dtT = ExecuteDt("select name from syscolumns where id=(select max(id) from sysobjects where xtype='u' and name='"+ tableName +"')");
            if (dtT == null)
                throw new Exception(string.Format("table {0} not exists in db.", tableName));

            using (SqlBulkCopy sqlBC = new SqlBulkCopy(tran.conn,SqlBulkCopyOptions.Default,tran.trans))
            {
                sqlBC.BatchSize = 1000;
                sqlBC.BulkCopyTimeout = 60;              
                sqlBC.DestinationTableName = tableName;
                foreach (DataColumn dc in dt.Columns)
                {
                    if (ExistTableColumn(dtT,dc.ColumnName))
                        sqlBC.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                sqlBC.WriteToServer(dt);
            }
        }

        bool ExistTableColumn(DataTable columnsTableResult, string columnName)
        {
            foreach (DataRow dr in columnsTableResult.Rows)
            {
                if (dr[0].ToString() == columnName)
                    return true;
            }
            return false;
        }

        public bool Exist(dynamic tran, string sqlstr)
        {
            var dt = ExecuteDt(sqlstr);
            return dt != null && dt.Rows.Count > 0;
        }

        public object ExecuteScalar(dynamic tran, string sqltxt)
        {
            lock (_lock)
            {
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sqltxt;
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="tran">BeginTransaction返回的对象</param>
        public void CommitTransaction(dynamic tran)
        {
            tran.trans.Commit();
            tran.conn.Close();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="tran">BeginTransaction返回的对象</param>
        public void RollbackTransaction(dynamic tran)
        {            
            tran.trans.Rollback();
            tran.conn.Close();
        }

        #endregion
    }
}
