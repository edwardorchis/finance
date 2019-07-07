using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Utils
{
    /// <summary>
    /// 日志打点接口。
    /// </summary>
    public interface ILogger
    {
        void Error(string message);
        void Warn(string message);
        void Info(string message);
    }
}
