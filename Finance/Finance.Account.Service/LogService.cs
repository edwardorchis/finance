using Finance.Account.Service.Model;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service
{


    public class LogService
    {
        Type m_type = null;  
        private IDictionary<string, object> mContext;
        public LogService(IDictionary<string, object> ctx, Type type)
        {
            mContext = ctx;
            m_type = type;
        }

        public static LogService GetInstance(IDictionary<string, object> ctx, Type type)
        {
            return new LogService(ctx, type);
        }

        public void Write(Operation operation,string message)
        {
            OperationLog log = new OperationLog();
            log.category = m_type.Name;
            log.operation = operation.ToString();
            log.username = mContext["UserName"].ToString();
            log.message = message;
            log.time = DateTime.Now;
            DataManager.GetInstance(mContext).Insert(log);
        }
    }
}
