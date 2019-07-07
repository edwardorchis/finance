using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public enum FinanceAccountDataErrorCode
    {
        SUCCESS = 0,
        /// <summary>
        /// 取消
        /// </summary>
        CANCEL,
        /// <summary>
        /// 不存在
        /// </summary>
        CACHE_DATA_NOT_EXIST,
        /// <summary>
        /// 
        /// </summary>
        DATA_NOT_EXIST,
        /// <summary>
        /// 数据格式化失败
        /// </summary>
        FORMAT_ERROR,
        /// <summary>
        /// 不支持
        /// </summary>
        NOT_SUPPORT,
        /// <summary>
        /// 未知的系统错误
        /// </summary>
        SYSTEM_ERROR,
    }

    public class FinanceAccountDataException : Exception
    {
        string msg = "";
        public FinanceAccountDataException(FinanceAccountDataErrorCode ErrCode, string Message = "")
        {
            HResult = (int)ErrCode;
            msg = Message;
        }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(msg))
                    return ((FinanceAccountDataErrorCode)HResult).ToString();
                else
                    return msg;
            }
        }
    }
}
